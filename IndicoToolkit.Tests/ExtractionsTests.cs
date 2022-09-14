using Xunit;
using IndicoToolkit.Types;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json.Linq;
using CsvHelper;

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

        [Fact]
        public void TestRemoveExceptMaxConfidence()
        {
            List<Prediction> predictions = new List<Prediction>();
            JObject newConfidence = new JObject{
                { "testLabel", .99f }
            };
            predictions.Add(Utils.CreatePrediction(text:"keep this", confidence: newConfidence));
            predictions.Add(Utils.CreatePrediction(text:"remove this"));
            Extractions extractionsObject = new Extractions(predictions);
            extractionsObject.removeExceptMaxConfidence(new List<string>(){"testLabel"});
            Assert.Equal(extractionsObject.Preds.Count, 1);
            Assert.Equal(extractionsObject.RemovedPreds.Count, 1);
            Assert.Equal((string)extractionsObject.Preds[0].getValue("text"), "keep this");
            Assert.Equal((string)extractionsObject.RemovedPreds[0].getValue("text"), "remove this");
        }

        [Fact]
        public void TestExistMultipleValsForLabel()
        {
            List<Prediction> predictions = new List<Prediction>();
            predictions.Add(Utils.CreatePrediction(text:"A"));
            predictions.Add(Utils.CreatePrediction(text:"B"));
            predictions.Add(Utils.CreatePrediction(label: "otherLabel", text: "C"));
            Extractions extractionsObject = new Extractions(predictions);
            Assert.True(extractionsObject.existMultipleValsForLabel("testLabel"));
            Assert.False(extractionsObject.existMultipleValsForLabel("otherLabel"));
        }

        [Fact]
        public void TestGetMostCommonTextValue()
        {
            List<Prediction> predictions = new List<Prediction>();
            predictions.Add(Utils.CreatePrediction(text:"this"));
            predictions.Add(Utils.CreatePrediction(text:"this"));
            predictions.Add(Utils.CreatePrediction(text:"not this"));
            Extractions extractionsObject = new Extractions(predictions);
            string mostCommonTextValue = extractionsObject.getMostCommonTextValue("testLabel");
            Assert.Equal(mostCommonTextValue, "this");
        }

        [Fact]
        public void TestToCSV()
        {
            List<Prediction> predictions = new List<Prediction>();
            predictions.Add(Utils.CreatePrediction(text:"A"));
            predictions.Add(Utils.CreatePrediction(text:"B"));
            string savePath = Path.Join(Utils.file_dir, "data/extractions/");
            string fileName = "extractions_to_csv.pdf";
            Extractions extractionsObject = new Extractions(predictions);
            extractionsObject.toCSV(savePath: savePath, fileName: fileName);
            using (var reader = new StreamReader(savePath + fileName))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                List<ExtractionRecord> records = csv.GetRecords<ExtractionRecord>().ToList();
                Assert.Equal(3, records.Count);
            }
        }

    }
}