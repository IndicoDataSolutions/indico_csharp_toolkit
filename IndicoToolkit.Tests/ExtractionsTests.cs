using Xunit;
using IndicoToolkit.Types;
using System;
using System.Collections.Generic;

namespace IndicoToolkit.Tests
{
    public class ExtractionFixture: IDisposable
    {
        public dynamic StaticExtractPreds { get; private set; }
        public List<Prediction> Predictions { get; private set; }

        public Extractions ExtractionsObject { get; private set; }
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

    }
    public class ExtractionsTests : IClassFixture<ExtractionFixture> 
    {

        ExtractionFixture Fixture { get; }

        public ExtractionsTests(ExtractionFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void TestInit()
        {
            Assert.Equal(Fixture.ExtractionsObject.Preds, Fixture.Predictions);
            Assert.Equal(Fixture.ExtractionsObject.RemovedPreds.Count, 0);
            Assert.IsType<Prediction>(Fixture.ExtractionsObject.Preds[0]);
        }

        [Fact]
        public void TestRemoveByConfidence()
        {
            List<Prediction> predictions = new List<Prediction>();
            for (int i = 0; i < Fixture.StaticExtractPreds.Count; i++) {
                predictions.Add(new Prediction(Fixture.StaticExtractPreds[i]));
            }
            Extractions extractionsObject = new Extractions(predictions);
            List<string> labels = new List<string>(){"Name", "Department"};
            extractionsObject.removeByConfidence(0.95f, labels);
            extractionsObject.removeByConfidence(0.9f);
            for (int i = 0; i < extractionsObject.Preds.Count; i++) {
                Prediction pred = extractionsObject.Preds[i];
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