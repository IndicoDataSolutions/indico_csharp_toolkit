using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace IndicoToolkit.Results;


public class Prediction : PrettyPrint
{
    public Document Document { get; init; }
    public ModelGroup Model { get; init; }
    public Review? Review { get; init; }  // Pre-review predictions do not have an associated Review.

    public string Label { get; set; }
    [NoPrint]
    public Dictionary<string, double> Confidences { get; init; }
    [NoPrint]
    public JObject Extras { get; init; }

    public double Confidence
    {
        get => Confidences[Label];
        set => Confidences[Label] = value;
    }

    // Create a Prediction subtype appropriate for `model.TaskType` from a prediction JSON.
    public static Prediction FromV1Json(Document document, ModelGroup model, Review? review, JToken json)
    {
        if (model.TaskType == TaskType.CLASSIFICATION)
            return Classification.FromV1Json(document, model, review, json);
        else if (model.TaskType == TaskType.DOCUMENT_EXTRACTION)
            return DocumentExtraction.FromV1Json(document, model, review, json);
        else if (model.TaskType == TaskType.FORM_EXTRACTION)
            return FormExtraction.FromV1Json(document, model, review, json);
        else
            throw new ResultException($"unsupported v1 task type `{model.TaskType}`");
    }

    // Create a Prediction subtype appropriate for `model.TaskType` from a prediction JSON.
    public static Prediction FromV3Json(Document document, ModelGroup model, Review? review, JToken json)
    {
        if (model.TaskType == TaskType.CLASSIFICATION)
            return Classification.FromV3Json(document, model, review, json);
        else if (model.TaskType == TaskType.DOCUMENT_EXTRACTION)
            return DocumentExtraction.FromV3Json(document, model, review, json);
        else if (model.TaskType == TaskType.FORM_EXTRACTION)
            return FormExtraction.FromV3Json(document, model, review, json);
        else if (model.TaskType == TaskType.UNBUNDLING)
            return Unbundling.FromV3Json(document, model, review, json);
        else
            throw new ResultException($"unsupported v3 task type `{model.TaskType}`");
    }

    public virtual JObject ToV1Json()
    {
        throw new NotImplementedException();
    }

    public virtual JObject ToV3Json()
    {
        throw new NotImplementedException();
    }
}
