using Xunit;
using System.Collections.Generic;
using IndicoToolkit.Types;
using Newtonsoft.Json.Linq;

namespace IndicoToolkit.Tests
{
    public class PredictionTests
    {
        static dynamic medExtractionsJson = Utils.LoadJson("data/samples/med_extractions.json");
        [Fact]
        public void Prediction_ImportJObject_ShouldImport()
        {
            List<Prediction> predictions = new List<Prediction>();
            foreach (var medExtraction in medExtractionsJson["results"]["document"]["results"]["Medical Record Extraction"]["pre_review"])
            {
                JObject medExtractionAsJObject = medExtraction as JObject;
                Prediction prediction = medExtractionAsJObject.ToObject<Prediction>();
                predictions.Add(prediction);
            }
            Assert.Equal(predictions[11].Groupings[0].GroupId, "820:Linked Label Group 1");
            Assert.Equal(predictions.Count, 44);
        }
    }
}