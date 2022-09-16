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
using CsvHelper.Configuration;

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
        public void RemoveByConfidence_StaticExtractPreds_True()
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
        public void RemoveExceptMaxConfidence_RemovesB_KeepsA()
        {
            List<Prediction> predictions = new List<Prediction>();
            predictions.Add(Utils.CreatePrediction(text:"A", confidence: Utils.CreateNewConfidence("testLabel", .99f)));
            predictions.Add(Utils.CreatePrediction(text:"B"));
            Extractions extractionsObject = new Extractions(predictions);
            extractionsObject.removeExceptMaxConfidence(new List<string>(){"testLabel"});
            Assert.Equal(extractionsObject.Preds.Count, 1);
            Assert.Equal(extractionsObject.RemovedPreds.Count, 1);
            Assert.Equal((string)extractionsObject.Preds[0].getValue("text"), "A");
            Assert.Equal((string)extractionsObject.RemovedPreds[0].getValue("text"), "B");
        }

        [Fact]
        public void ExistMultipleValsForLabel_Basic_True()
        {
            List<Prediction> predictions = new List<Prediction>();
            predictions.Add(Utils.CreatePrediction(text:"A"));
            predictions.Add(Utils.CreatePrediction(text:"B"));
            Extractions extractionsObject = new Extractions(predictions);
            Assert.True(extractionsObject.existMultipleValsForLabel("testLabel"));
        }

        [Fact]
        public void ExistMultipleValsForLabel_DiffConfidences_True()
        {
            List<Prediction> predictions = new List<Prediction>();
            predictions.Add(Utils.CreatePrediction(text:"A"));
            predictions.Add(Utils.CreatePrediction(text:"B", confidence: Utils.CreateNewConfidence("testLabel", .99f)));
            predictions.Add(Utils.CreatePrediction(label: "otherLabel", text: "C"));
            Extractions extractionsObject = new Extractions(predictions);
            Assert.True(extractionsObject.existMultipleValsForLabel("testLabel"));
            Assert.False(extractionsObject.existMultipleValsForLabel("otherLabel"));
        }

        [Fact]
        public void ExistMultipleValsForLabel_OneVal_False()
        {
            List<Prediction> predictions = new List<Prediction>();
            predictions.Add(Utils.CreatePrediction(label: "otherLabel", text: "C"));
            Extractions extractionsObject = new Extractions(predictions);
            Assert.False(extractionsObject.existMultipleValsForLabel("otherLabel"));
        }

        [Fact]
        public void GetMostCommonTextValue_ReturnA()
        {
            List<Prediction> predictions = new List<Prediction>();
            predictions.Add(Utils.CreatePrediction(text:"A"));
            predictions.Add(Utils.CreatePrediction(text:"A"));
            predictions.Add(Utils.CreatePrediction(text:"B"));
            Extractions extractionsObject = new Extractions(predictions);
            string mostCommonTextValue = extractionsObject.getMostCommonTextValue("testLabel");
            Assert.Equal(mostCommonTextValue, "A");
        }

        [Fact]
        public void GetMostCommonTextValue_Tie_ReturnNull()
        {
            List<Prediction> predictions = new List<Prediction>();
            predictions.Add(Utils.CreatePrediction(text:"A"));
            predictions.Add(Utils.CreatePrediction(text:"A"));
            predictions.Add(Utils.CreatePrediction(text:"B"));
            predictions.Add(Utils.CreatePrediction(text:"B"));
            Extractions extractionsObject = new Extractions(predictions);
            string mostCommonTextValue = extractionsObject.getMostCommonTextValue("testLabel");
            Assert.Null(mostCommonTextValue);
        }

        [Fact]
        public void ToCSV_IncludeStartEndFalse_ThreeColumnOutput()
        {
            List<Prediction> predictions = new List<Prediction>();
            predictions.Add(Utils.CreatePrediction(text:"A"));
            predictions.Add(Utils.CreatePrediction(text:"B"));
            string savePath = Path.Join(Utils.file_dir, "data/extractions/");
            Extractions extractionsObject = new Extractions(predictions);
            string fileName = "extractions.csv";
            extractionsObject.toCSV(savePath: savePath, fileName: fileName);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                HasHeaderRecord = false
            };
            using (var reader = new StreamReader(savePath + fileName))
            using (var csv = new CsvReader(reader, config))
            {
                List<ExtractionRecord> records = csv.GetRecords<ExtractionRecord>().ToList();
                Assert.Equal(3, records.Count);
            }
        }

        [Fact]
        public void ToCSV_IncludeStartEndTrue_FiveColumnOutput()
        {
            List<Prediction> predictions = new List<Prediction>();
            predictions.Add(Utils.CreatePrediction(text:"A"));
            predictions.Add(Utils.CreatePrediction(text:"B"));
            Extractions extractionsObject = new Extractions(predictions);
            string savePath = Path.Join(Utils.file_dir, "data/extractions/");
            string fileName = "extractions_include_start_end.csv";
            extractionsObject.toCSV(savePath: savePath, fileName: fileName, includeStartEnd: true);
            using (var reader = new StreamReader(savePath + fileName))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                List<ExtractionRecord> records = csv.GetRecords<ExtractionRecord>().ToList();
                Assert.Equal(2, records.Count);
                int numColumns = csv.HeaderRecord.Count();
                Assert.Equal(5, numColumns);
            }
        }

    }
}