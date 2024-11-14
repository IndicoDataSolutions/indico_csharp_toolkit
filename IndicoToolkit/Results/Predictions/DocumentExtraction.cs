using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace IndicoToolkit.Results;


public class DocumentExtraction : Extraction
{
    public int Start { get; set; }
    public int End { get; set; }

    // Create an DocumentExtraction from a v1 prediction object.
    public static DocumentExtraction FromV1Json(Document document, ModelGroup model, Review? review, JToken json)
    {
        var normalized = Utils.Get<JObject>(json, "normalized");

        return new DocumentExtraction
        {
            Document = document,
            Model = model,
            Review = review,
            Label = Utils.Get<string>(json, "label"),
            Confidences = Utils.Get<Dictionary<string, double>>(json, "confidence"),
            Accepted = Utils.Has<bool>(json, "accepted") && Utils.Get<bool>(json, "accepted"),
            Rejected = Utils.Has<bool>(json, "rejeted") && Utils.Get<bool>(json, "rejeted"),
            Text = Utils.Get<string>(normalized, "formatted"),
            Page = Utils.Get<int>(json, "page_num"),
            Start = Utils.Get<int>(json, "start"),
            End = Utils.Get<int>(json, "end"),
            Extras = json as JObject,
        };
    }

    // Create an DocumentExtraction from a v3 prediction object.
    public static DocumentExtraction FromV3Json(Document document, ModelGroup model, Review? review, JToken json)
    {
        var normalized = Utils.Get<JObject>(json, "normalized");
        var span = Utils.Get<JArray>(json, "spans").First;

        return new DocumentExtraction
        {
            Document = document,
            Model = model,
            Review = review,
            Label = Utils.Get<string>(json, "label"),
            Confidences = Utils.Get<Dictionary<string, double>>(json, "confidence"),
            Accepted = Utils.Has<bool>(json, "accepted") && Utils.Get<bool>(json, "accepted"),
            Rejected = Utils.Has<bool>(json, "rejeted") && Utils.Get<bool>(json, "rejeted"),
            Text = Utils.Get<string>(normalized, "formatted"),
            Page = Utils.Get<int>(span, "page_num"),
            Start = Utils.Get<int>(span, "start"),
            End = Utils.Get<int>(span, "end"),
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
