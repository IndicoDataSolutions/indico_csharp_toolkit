using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace IndicoToolkit.Results;


public enum FormExtractionType
{
    CHECKBOX,
    SIGNATURE,
    TEXT
}


public class FormExtraction : AutoReviewable
{
    public FormExtractionType Type { get; set; }
    public bool Checked { get; set; }
    public bool Signed { get; set; }
    public string Text { get; set; }

    public ulong Page { get; set; }
    public ulong Top { get; set; }
    public ulong Left { get; set; }
    public ulong Right { get; set; }
    public ulong Bottom { get; set; }

    // Determine the form extraction type of a prediction from its string representation.
    public static FormExtractionType FormExtractionTypeFromString(string formExtractionType)
    {
        if (formExtractionType == "checkbox")
            return FormExtractionType.CHECKBOX;
        else if (formExtractionType == "signature")
            return FormExtractionType.SIGNATURE;
        else if (formExtractionType == "text")
            return FormExtractionType.TEXT;
        else
            throw new ResultException($"unsupported form extraction type `{formExtractionType}`");
    }

    // Create a FormExtraction from a prediction JSON.
    public static FormExtraction _FromJson(Document document, ModelGroup model, Review? review, JToken json)
    {
        var normalized = Utils.Get<JObject>(json, "normalized");
        var structured = Utils.Get<JObject>(normalized, "structured");

        return new FormExtraction
        {
            Document = document,
            Model = model,
            Review = review,
            Label = Utils.Get<string>(json, "label"),
            Confidences = Utils.Get<Dictionary<string, double>>(json, "confidence"),
            Accepted = Utils.Has<bool>(json, "accepted") && Utils.Get<bool>(json, "accepted"),
            Rejected = Utils.Has<bool>(json, "rejected") && Utils.Get<bool>(json, "rejected"),
            Type = FormExtractionTypeFromString(Utils.Get<string>(json, "type")),
            Checked = Utils.Has<bool>(structured, "checked") && Utils.Get<bool>(structured, "checked"),
            Signed = Utils.Has<bool>(structured, "signed") && Utils.Get<bool>(structured, "signed"),
            Text = Utils.Get<string>(normalized, "formatted"),
            Page = Utils.Get<ulong>(json, "page_num"),
            Top = Utils.Get<ulong>(json, "top"),
            Left = Utils.Get<ulong>(json, "left"),
            Right = Utils.Get<ulong>(json, "right"),
            Bottom = Utils.Get<ulong>(json, "bottom"),
            Extras = json as JObject,
        };
    }

    public static FormExtraction FromV1Json(Document document, ModelGroup model, Review? review, JToken json)
    {
        return _FromJson(document, model, review, json);
    }

    public static FormExtraction FromV3Json(Document document, ModelGroup model, Review? review, JToken json)
    {
        return _FromJson(document, model, review, json);
    }

    public JObject _ToJson()
    {
        Extras["label"] = Label;
        Extras["confidence"] = JObject.FromObject(Confidences);
        Extras["type"] = Type.ToString().ToLower();
        Extras["page_num"] = Page;
        Extras["top"] = Top;
        Extras["left"] = Left;
        Extras["right"] = Right;
        Extras["bottom"] = Bottom;

        if (Type == FormExtractionType.CHECKBOX)
        {
            Extras["normalized"]["structured"]["checked"] = Checked;
            Extras["normalized"]["formatted"] = Checked ? "Checked" : "Unchecked";
        }
        else if (Type == FormExtractionType.SIGNATURE)
        {
            Extras["normalized"]["structured"]["signed"] = Signed;
            Extras["normalized"]["formatted"] = Signed ? "Signed" : "Unsigned";
        }
        else if (Type == FormExtractionType.TEXT)
        {
            Extras["normalized"]["formatted"] = Text;
        }

        if (Accepted)
            Extras["accepted"] = true;
        else if (Rejected)
            Extras["rejected"] = true;

        return Extras;
    }

    public override JObject ToV1Json()
    {
        return _ToJson();
    }

    public override JObject ToV3Json()
    {
        return _ToJson();
    }
}
