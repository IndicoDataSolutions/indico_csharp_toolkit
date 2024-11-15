using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace IndicoToolkit.Results;


public class Unbundling : Prediction
{
    public List<int> Pages { get; set; }

    // Create a Unbundling from a v3 prediction JSON.
    public static Unbundling FromV3Json(Document document, ModelGroup model, Review? review, JToken json)
    {
        var spans = Utils.Get<JArray>(json, "spans");

        return new Unbundling
        {
            Document = document,
            Model = model,
            Review = review,
            Label = Utils.Get<string>(json, "label"),
            Confidences = Utils.Get<Dictionary<string, double>>(json, "confidence"),
            Pages = spans.Select(span => Utils.Get<int>(span, "page_num")).ToList(),
            Extras = json as JObject,
        };
    }

    public override JObject ToV3Json()
    {
        Extras["label"] = Label;
        Extras["confidence"] = JObject.FromObject(Confidences);
        Extras["spans"] = new JArray(Pages.Select(page => new JObject { ["page_num"] = page }));

        return Extras;
    }
}
