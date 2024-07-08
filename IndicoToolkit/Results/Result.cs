using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace IndicoToolkit.Results;


public class Result : PrettyPrint
{
    public ulong Id { get; init; }
    public ulong Version { get; init; }
    public List<Document> Documents { get; init; }
    public List<ModelGroup> Models { get; init; }
    public PredictionList<Prediction> Predictions { get; init; }
    public List<Review> Reviews { get; init; }

    [NoPrint]
    public bool Rejected => Reviews.Any() && Reviews.Last().Rejected;
    [NoPrint]
    public PredictionList<Prediction> PreReview => Predictions.Where(pred => pred.Review == null);
    [NoPrint]
    public PredictionList<Prediction> AutoReview => Predictions.Where(reviewType: ReviewType.AUTO);
    [NoPrint]
    public PredictionList<Prediction> ManualReview => Predictions.Where(reviewType: ReviewType.MANUAL);
    [NoPrint]
    public PredictionList<Prediction> AdminReview => Predictions.Where(reviewType: ReviewType.ADMIN);
    [NoPrint]
    public PredictionList<Prediction> Final => Predictions.Where(pred => pred.Review == (Reviews.Any() ? Reviews.Last() : null));

    // Create a Result from the root object of a result file.
    public static Result FromJson(JObject json)
    {
        var version = Utils.Get<ulong>(json, "file_version");

        if (version == 1)
            return FromV1Json(json);
        else if (version == 3)
            return FromV3Json(json);
        else
            throw new ResultException($"unsupported file version `{version}`");
    }

    // Create a Result from the root object of a v1 result file.
    private static Result FromV1Json(JObject json)
    {
        NormalizeV1Json(json);

        var id = Utils.Get<ulong>(json, "submission_id");
        var version = Utils.Get<ulong>(json, "file_version");
        var document = Document.FromV1Json(json);  // v1 results support only 1 document.
        var documents = new PrettyPrintList<Document> { document };
        var models = new PrettyPrintList<ModelGroup>();
        var predictions = new PredictionList<Prediction>();

        // Reviews must be sorted after parsing predictions, as they match positionally
        // with prediction lists in `post_reviews`.
        var reviews = new PrettyPrintList<Review>(
            Utils.Get<JArray>(json, "reviews_meta")
                .Select(value => Review.FromJson(value))
        );

        var results = Utils.Get<JObject>(Utils.Get<JObject>(Utils.Get<JObject>(json, "results"), "document"), "results");

        foreach (var modelJson in results)
        {
            var model = ModelGroup.FromV1Json(modelJson);
            models.Add(model);
            // Track model sections so empty ones can be reproduced in auto review changes.
            document.ModelSections.Add(model.Name);

            predictions.AddRange(
                Utils.Get<JArray>(modelJson.Value, "pre_review")
                    .Select(predictionJson => Prediction.FromV1Json(
                        document, model, review: null, predictionJson
                    ))
            );
            foreach (var tuple in reviews.Zip(Utils.Get<JArray>(modelJson.Value, "post_reviews"))
                                    .Where(tuple => !tuple.First.Rejected))  // Skip rejected review sections as their associated
            {                                                                // prediction sections are null.
                predictions.AddRange(
                    tuple.Second.Select(predictionJson => Prediction.FromV1Json(
                        document, model, tuple.First, predictionJson
                    ))
                );
            }
        }

        reviews.Sort((left, right) => left.Id.CompareTo(right.Id));

        return new Result
        {
            Id = id,
            Version = version,
            Documents = documents,
            Models = models,
            Predictions = predictions,
            Reviews = reviews,
        };
    }

    // Fix inconsistencies observed in v1 result files.
    private static void NormalizeV1Json(JObject json)
    {
        var submissionResults = json["results"]["document"]["results"] as JObject;

        // Incomplete and unreviewed submissions don't include a `reviews_meta` section.
        if (!Utils.Has<JArray>(json, "reviews_meta"))
        {
            json["reviews_meta"] = new JArray();

            foreach (var modelResults in submissionResults)
            {
                JToken predictions = modelResults.Value;
                submissionResults[modelResults.Key] = new JObject();
                submissionResults[modelResults.Key]["pre_review"] = predictions;
                submissionResults[modelResults.Key]["post_reviews"] = new JArray();
            }
        }

        // Incomplete and unreviewed submissions retrieved with `SubmissionResult()`
        // have a single `{"review_id": null}` review.
        if (json["reviews_meta"].Count() > 0 && !Utils.Has<int>(json["reviews_meta"][0], "review_id"))
            json["reviews_meta"] = new JArray();

        // Review notes are `null` unless the reviewer enters a reason for rejection.
        for (var index = 0; index < json["reviews_meta"].Count(); index++)
            if (!Utils.Has<string>(json["reviews_meta"][index], "review_notes"))
                json["reviews_meta"][index]["review_notes"] = "";

        // Classifications aren't wrapped in lists like all other prediction types.
        foreach (var modelResults in submissionResults)
        {
            var predictions = modelResults.Value;

            if (Utils.Has<JObject>(predictions, "pre_review"))
            {
                predictions["pre_review"] = new JArray { predictions["pre_review"] };

                for (var index = 0; index < predictions["post_reviews"].Count(); index++)
                    predictions["post_reviews"][index] = new JArray { predictions["post_reviews"][index] };
            }
        }
    }

    // Create a Result from the root object of a v3 result file.
    private static Result FromV3Json(JObject json)
    {
        NormalizeV3Json(json);

        var id = Utils.Get<ulong>(json, "submission_id");
        var version = Utils.Get<ulong>(json, "file_version");
        var documents = new PrettyPrintList<Document>();
        var models = new PrettyPrintList<ModelGroup>(
            Utils.Get<JObject>(json, "modelgroup_metadata")
                .PropertyValues()
                .Select(value => ModelGroup.FromV3Json(value))
                .OrderBy(model => model.Id ?? 0)  // v3 Model Group IDs won't actually be null in practice.
        );
        var predictions = new PredictionList<Prediction>();
        var reviews = new PrettyPrintList<Review>(
            Utils.Get<JObject>(json, "reviews")
                .PropertyValues()
                .Select(value => Review.FromJson(value))
                .OrderBy(review => review.Id)
        );

        foreach (var documentJson in Utils.Get<JArray>(json, "submission_results"))
        {
            var document = Document.FromV3Json(documentJson);
            documents.Add(document);

            var modelResultsJson = Utils.Get<JObject>(documentJson, "model_results");
            var originalJson = Utils.Get<JObject>(modelResultsJson, "ORIGINAL");
            // Unreviewed results do not have a `FINAL` section.

            // Parse pre-review predictions (which don't have an associated review).
            foreach (var modelJson in originalJson)
            {
                var modelId = ulong.Parse(modelJson.Key);
                var model = models.Where(model => model.Id == modelId).First();

                foreach (var predictionJson in modelJson.Value as JArray)
                    predictions.Add(Prediction.FromV3Json(
                        document, model, review: null, predictionJson
                    ));

                // Track model sections so empty ones can be reproduced in auto review changes.
                document.ModelSections.Add(modelJson.Key);
            }

            // Parse final predictions (which don't have an associated review).
            if (reviews.Any())
            {
                var review = reviews.Last();
                var finalJson = Utils.Get<JObject>(modelResultsJson, "FINAL");

                foreach (var modelJson in finalJson)
                {
                    var modelId = ulong.Parse(modelJson.Key);
                    var model = models.Where(model => model.Id == modelId).First();

                    foreach (var predictionJson in modelJson.Value as JArray)
                        predictions.Add(Prediction.FromV3Json(
                            document, model, review, predictionJson
                        ));
                }
            }
        }

        documents.Sort((left, right) => (left.Id ?? 0).CompareTo(right.Id ?? 0));

        return new Result
        {
            Id = id,
            Version = version,
            Documents = documents,
            Models = models,
            Predictions = predictions,
            Reviews = reviews,
        };
    }

    // Fix inconsistencies observed in v3 result files.
    private static void NormalizeV3Json(JObject json)
    {
        // Prior to 6.8, v3 result files don't include a `reviews` section.
        if (!Utils.Has<JObject>(json, "reviews"))
            json["reviews"] = new JObject();

        // Review notes are `null` unless the reviewer enters a reason for rejection.
        foreach (var review in json["reviews"] as JObject)
            if (!Utils.Has<string>(review.Value, "review_notes"))
                review.Value["review_notes"] = "";
    }
}
