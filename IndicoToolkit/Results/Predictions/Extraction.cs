using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace IndicoToolkit.Results;


public class Extraction : AutoReviewable
{
    public string Text { get; set; }
    public ulong Page { get; set; }
    public ulong Start { get; set; }
    public ulong End { get; set; }

    // Create an Extraction from a v1 prediction object.
    public static Extraction FromV1Json(Document document, ModelGroup model, Review? review, JToken json)
    {
        var normalized = Utils.Get<JObject>(json, "normalized");

        return new Extraction
        {
            Document = document,
            Model = model,
            Review = review,
            Label = Utils.Get<string>(json, "label"),
            Confidences = Utils.Get<Dictionary<string, double>>(json, "confidence"),
            Accepted = Utils.Has<bool>(json, "accepted") && Utils.Get<bool>(json, "accepted"),
            Rejected = Utils.Has<bool>(json, "rejeted") && Utils.Get<bool>(json, "rejeted"),
            Text = Utils.Get<string>(normalized, "formatted"),
            Page = Utils.Get<ulong>(json, "page_num"),
            Start = Utils.Get<ulong>(json, "start"),
            End = Utils.Get<ulong>(json, "end"),
            Extras = json as JObject,
        };
    }

    // Create an Extraction from a v3 prediction object.
    public static Extraction FromV3Json(Document document, ModelGroup model, Review? review, JToken json)
    {
        var normalized = Utils.Get<JObject>(json, "normalized");
        var span = Utils.Get<JArray>(json, "spans").First;

        return new Extraction
        {
            Document = document,
            Model = model,
            Review = review,
            Label = Utils.Get<string>(json, "label"),
            Confidences = Utils.Get<Dictionary<string, double>>(json, "confidence"),
            Accepted = Utils.Has<bool>(json, "accepted") && Utils.Get<bool>(json, "accepted"),
            Rejected = Utils.Has<bool>(json, "rejeted") && Utils.Get<bool>(json, "rejeted"),
            Text = Utils.Get<string>(normalized, "formatted"),
            Page = Utils.Get<ulong>(span, "page_num"),
            Start = Utils.Get<ulong>(span, "start"),
            End = Utils.Get<ulong>(span, "end"),
            Extras = json as JObject,
        };
    }

    public override JObject ToV1Json()
    {
        Extras["label"] = Label;
        Extras["confidence"] = JObject.FromObject(Confidences);
        Extras["normalized"]["formatted"] = Text;
        Extras["page_num"] = Page;
        Extras["start"] = Start;
        Extras["end"] = End;

        if (Accepted)
            Extras["accepted"] = true;
        else if (Rejected)
            Extras["rejeted"] = true;

        return Extras;
    }

    public override JObject ToV3Json()
    {
        Extras["label"] = Label;
        Extras["confidence"] = JObject.FromObject(Confidences);
        Extras["normalized"]["formatted"] = Text;
        Extras["spans"][0]["page_num"] = Page;
        Extras["spans"][0]["start"] = Start;
        Extras["spans"][0]["end"] = End;

        if (Accepted)
            Extras["accepted"] = true;
        else if (Rejected)
            Extras["rejeted"] = true;

        return Extras;
    }
}
