using Xunit;
using IndicoToolkit;
using IndicoToolkit.Types;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

namespace IndicoToolkit.Tests
{
    public class ExtractionsTests 
    {
        [Fact]
        public void TestInit()
        {
            dynamic predictions = Utils.LoadJson("data/samples/fin_disc_result.json")["results"]["document"]["results"]["Toolkit Test Financial Model"];
            List<Prediction> staticExtractPreds = new List<Prediction>();
            for (int i = 0; i < predictions.Count; i++) {
                staticExtractPreds.Add(new Prediction(predictions[i]));
            }
            Extractions extractions = new Extractions(staticExtractPreds);
            Assert.Equal(extractions.Preds(), staticExtractPreds);
            Assert.Equal(extractions.removed_predictions.Count, 0);
            Assert.IsType<Prediction>(extractions.Preds()[0]);
        }

    }
}