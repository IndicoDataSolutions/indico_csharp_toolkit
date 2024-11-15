using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace IndicoToolkit.Results;


public class Classification : Prediction
{
    // Create a Classification from a prediction JSON.
    public static Classification _FromJson(Document document, ModelGroup model, Review? review, JToken json)
    {
        return new Classification
        {
            Document = document,
            Model = model,
            Review = review,
            Label = Utils.Get<string>(json, "label"),
            Confidences = Utils.Get<Dictionary<string, double>>(json, "confidence"),
            Extras = json as JObject,
        };
    }

    public static Classification FromV1Json(Document document, ModelGroup model, Review? review, JToken json)
    {
        return _FromJson(document, model, review, json);
    }

    public static Classification FromV3Json(Document document, ModelGroup model, Review? review, JToken json)
    {
        return _FromJson(document, model, review, json);
    }

    // Create JSON for auto review changes.
    public JObject _ToJson()
    {
        Extras["label"] = Label;
        Extras["confidence"] = JObject.FromObject(Confidences);

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
