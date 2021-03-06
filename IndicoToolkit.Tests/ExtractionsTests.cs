using Xunit;
using IndicoToolkit;
using IndicoToolkit.Types;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

namespace IndicoToolkit.Tests
{
    public class ExtractionFixture: IDisposable
    {
        public ExtractionFixture()
        {
            StaticExtractPreds = Utils.LoadJson("data/samples/fin_disc_result.json")["results"]["document"]["results"]["Toolkit Test Financial Model"];
            Predictions = new List<Prediction>();
            for (int i = 0; i < StaticExtractPreds.Count; i++) {
                Predictions.Add(new Prediction(StaticExtractPreds[i]));
            }
            ExtractionsObject = new Extractions(Predictions);
        }

        public void Dispose()
        {
            // ... clean up test data if needed
        }

        public dynamic StaticExtractPreds { get; private set; }
        public List<Prediction> Predictions { get; private set; }

        public Extractions ExtractionsObject { get; private set; }
    }
    public class ExtractionsTests : IClassFixture<ExtractionFixture> 
    {

        ExtractionFixture fixture;

        public ExtractionsTests(ExtractionFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void TestInit()
        {
            Assert.Equal(fixture.ExtractionsObject.getPreds(), fixture.Predictions);
            Assert.Equal(fixture.ExtractionsObject.removedPredictions.Count, 0);
            Assert.IsType<Prediction>(fixture.ExtractionsObject.getPreds()[0]);
        }

        [Fact]
        public void TestRemoveByConfidence()
        {
            List<Prediction> predictions = new List<Prediction>();
            for (int i = 0; i < fixture.StaticExtractPreds.Count; i++) {
                predictions.Add(new Prediction(fixture.StaticExtractPreds[i]));
            }
            Extractions extractionsObject = new Extractions(predictions);
            List<string> labels = new List<string>(){"Name", "Department"};
            extractionsObject.removeByConfidence(0.95f, labels);
            extractionsObject.removeByConfidence(0.9f);
            for (int i = 0; i < extractionsObject.getPreds().Count; i++) {
                Prediction pred = extractionsObject.getPreds()[i];
                string pred_label = pred.getValue("label");
                if (labels.Contains(pred_label)) {
                    Assert.True(pred.getValue("confidence")[pred_label] > 0.95f);
                } else {
                    Assert.True(pred.getValue("confidence")[pred_label] > 0.9f);
                }
            }
        }

    }
}