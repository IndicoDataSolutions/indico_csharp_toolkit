using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CsvHelper;

namespace IndicoToolkit.Types
{
    /// <summary>
    /// Class <c>Extractions</c> provides functionality for common extraction prediction use cases.
    /// </summary>
    public class Extractions
    {
        public List<Prediction> Preds { get; private set; }
        public List<Prediction> RemovedPreds { get; private set; }
        public Dictionary<string, List<Prediction>> PredictionLabelMap { get; private set; }
        public HashSet<string> LabelSet { get; private set; }

        public Extractions(List<Prediction> predictions)
        {
            Preds = predictions;
            RemovedPreds = new List<Prediction>();
            PredictionLabelMap = toDictByLabel();
            LabelSet = getLabelSet(predictions);
        }

        /// <summary>
        /// Get the included labels from a list of predictions
        /// </summary>
        private HashSet<string> getLabelSet(List<Prediction> predictions)
        {
            HashSet<string> labelSet = new HashSet<string>();
            for (int i = 0; i < predictions.Count; i++)
            {
                labelSet.Add(predictions[i].Label);
            }
            return labelSet;
        }

        /// <summary>
        /// Method <c>toDictByLabel</c> generates a dictionary where key is label string and value is list of all predictions of that label.
        /// </summary>
        public Dictionary<string, List<Prediction>> toDictByLabel()
        {
            Dictionary<string, List<Prediction>> predictionLabelMap = new Dictionary<string, List<Prediction>>();
            for (int i = 0; i < Preds.Count; i++)
            {
                Prediction pred = Preds[i];
                string predLabel = pred.Label;

                if (predictionLabelMap.ContainsKey(predLabel))
                {
                    predictionLabelMap[predLabel].Add(pred);
                }
                else
                {
                    predictionLabelMap.Add(predLabel, new List<Prediction>() { pred });
                }
            }
            return predictionLabelMap;
        }

        /// <summary>
        /// Method <c>numPredictions</c> returns number of predictions.
        /// </summary>
        public int numPredictions()
        {
            return Preds.Count;
        }

        /// <summary>
        /// Method <c>removeByConfidence</c> removes predictions that are less than given confidence.
        /// <param name="confidence"> Confidence threshold. Defaults to 0.95f. </param>
        /// <param name="labels"> Labels where this applies. If null, applies to all. Defaults to null. </param>
        /// </summary>
        public void removeByConfidence(float confidence = 0.95f, List<string> labels = null)
        {
            List<Prediction> highConfPreds = new List<Prediction>();
            for (int i = 0; i < Preds.Count; i++)
            {
                Prediction pred = Preds[i];
                string predLabel = pred.Label;
                if (labels is not null && !labels.Contains(predLabel))
                {
                    highConfPreds.Add(pred);
                }
                else if (pred.Confidence[predLabel] >= confidence)
                {
                    highConfPreds.Add(pred);
                }
                else
                {
                    RemovedPreds.Add(pred);
                }
            }
            Preds = highConfPreds;
        }

        /// <summary>
        /// Method <c>removeExceptMaxConfidence</c> removes all predictions except the highest confidence within each specified class.
        /// </summary>
        public void removeExceptMaxConfidence(List<string> labels)
        {
            for (int i = 0; i < labels.Count; i++)
            {
                string label = labels[i];
                if (LabelSet.Contains(label))
                {
                    Prediction maxPred = selectMaxConfidence(label);
                    List<string> labelList = new List<string>() { label };
                    removeAllByLabel(labelList);
                    RemovedPreds.Remove(maxPred);
                    Preds.Add(maxPred);
                }
            }
        }

        /// <summary>
        /// Creates deep copy of list of predictions.
        /// </summary>
        private List<Prediction> DeepCopyPredictions(List<Prediction> preds)
        {
            List<Prediction> clone = new List<Prediction>();
            foreach (Prediction pred in preds)
            {
                Prediction newPred = JsonConvert.DeserializeObject<Prediction>(
                JsonConvert.SerializeObject(pred));
                clone.Add(newPred);
            }
            return clone;
        }

        /// <summary>
        /// Overwrite confidence dictionary to just max confidence float to make preds more readable.
        /// </summary>
        /// NOTE: Deep cloning is currently broken.
        public List<Prediction> setConfidenceKeyToMaxValue(bool inplace = true)
        {
            if (inplace)
            {
                return SetConfidenceKeyToMaxValue(Preds);
            }
            else
            {
                return SetConfidenceKeyToMaxValue(DeepCopyPredictions(Preds));
            }
        }

        private static List<Prediction> SetConfidenceKeyToMaxValue(List<Prediction> preds)
        {
            for (int i = 0; i < preds.Count; i++)
            {
                Prediction pred = preds[i];
                float maxConfidence = pred.Confidence[pred.Label];
                pred.Confidence = new Dictionary<string, float>() { { pred.Label, maxConfidence } };
            }
            return preds;
        }

        /// <summary>
        /// Method <c>removeKeys</c> removes specified keys from prediction dictionaries.
        /// </summary>
        public void removeKeys(List<string> keysToRemove = default(List<string>))
        {
            if (keysToRemove is null)
            {
                keysToRemove = new List<string>() { "start", "end" };
            }
            for (int i = 0; i < Preds.Count; i++)
            {
                Prediction pred = Preds[i];
                for (int j = 0; j < keysToRemove.Count; j++)
                {
                    string key = keysToRemove[j];
                    //pred.removeKey(key);
                }
            }
        }

        /// <summary>
        /// Method <c>removeAllByLabel</c> removes all predictions in list of labels.
        /// </summary>
        public void removeAllByLabel(List<string> labels)
        {
            List<Prediction> newPreds = new List<Prediction>();
            for (int i = 0; i < Preds.Count; i++)
            {
                Prediction pred = Preds[i];
                if (labels.Contains((string)pred.Label))
                {
                    RemovedPreds.Add(pred);
                }
                else
                {
                    newPreds.Add(pred);
                }
            }
            Preds = newPreds;
        }

        /// <summary>
        /// Method <c>removeNonIndexedPrediction</c> removes predictions that don't have position indexes.
        /// </summary>
        public void removeNonIndexedPredictions()
        {
            List<Prediction> newPreds = new List<Prediction>();
            for (int i = 0; i < Preds.Count; i++)
            {
                Prediction pred = Preds[i];
                if (!isManuallyAddedPrediction(pred))
                {
                    newPreds.Add(pred);
                }
            }
            Preds = newPreds;
        }

        /// <summary>
        /// Method <c>isManuallyAddedPrediction</c> checks if prediction has been manually added.
        /// </summary>
        public static bool isManuallyAddedPrediction(Prediction prediction)
        {

            if (prediction.End > prediction.Start)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method <c>selectMaxConfidence</c> gets the highest confidence prediction for a given field.
        /// </summary>
        public Prediction selectMaxConfidence(string label)
        {
            Prediction maxPred = null;
            float confidence = 0f;
            List<Prediction> preds = PredictionLabelMap[label];
            for (int i = 0; i < preds.Count; i++)
            {
                Prediction pred = preds[i];
                string predLabel = pred.Label;
                float predConfidence = pred.Confidence[predLabel];
                if (predConfidence >= confidence)
                {
                    maxPred = pred;
                    confidence = predConfidence;
                }
            }
            return maxPred;
        }

        /// <summary>
        /// Check whether there are multiple unique text values for field
        /// </summary>
        public bool existMultipleValsForLabel(string label)
        {
            List<Prediction> preds = PredictionLabelMap[label];
            List<string> predictionTextValues = new List<string>();
            foreach (Prediction pred in preds)
            {
                if (!predictionTextValues.Contains((string)pred.Text))
                {
                    predictionTextValues.Add((string)pred.Text);
                }
            }
            if (predictionTextValues.Count > 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return the most common text value. If there is a tie- returns null;
        /// </summary>
        public string getMostCommonTextValue(string label)
        {
            List<Prediction> predictions = PredictionLabelMap[label];
            Dictionary<string, int> textToCount = new Dictionary<string, int>();
            for (int i = 0; i < predictions.Count; i++)
            {
                string text = predictions[i].Text;
                if (textToCount.ContainsKey(text))
                {
                    textToCount[text] += 1;
                }
                else
                {
                    textToCount[text] = 1;
                }
            }
            string mostCommonText = textToCount.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            int mostCommonTextCount = textToCount[mostCommonText];
            int count = textToCount.Values.Where(x => x.Equals(mostCommonTextCount)).Count();
            /// Check for ties
            if (count > 1)
            {
                return null;
            }
            return mostCommonText;
        }

        /// <summary>
        /// Method <c>toCSV</c> writes three column CSV ('confidence', 'label', 'text')
        /// <param name="savePath"> Path to write CSV </param>
        /// <param name="fileName"> The file where the preds were derived from. Defaults to "". </param>
        /// <param name="includeStartEnd"> Include columns for start/end indexes. Defaults to false. </param>
        /// </summary>
        public void toCSV(string savePath, string fileName = "", bool includeStartEnd = false)
        {
            List<Prediction> preds = setConfidenceKeyToMaxValue(inplace: false);
            List<ExtractionRecord> ExtractionRecords = new List<ExtractionRecord>();
            for (int i = 0; i < preds.Count; i++)
            {
                Prediction pred = preds[i];
                ExtractionRecords.Add(new ExtractionRecord
                {
                    Start = includeStartEnd ? pred.Start.ToString() : null,
                    End = includeStartEnd ? pred.End.ToString() : null,
                    Label = pred.Label,
                    Text = pred.Text,
                    Confidence = pred.Confidence[pred.Label].ToString()
                });
            }
            string json = JsonConvert.SerializeObject(ExtractionRecords, Formatting.Indented);
            var expandos = JsonConvert.DeserializeObject<ExpandoObject[]>(json);

            using (var writer = new StringWriter())
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(expandos as IEnumerable<dynamic>);
                }
                File.WriteAllText(savePath + fileName, writer.ToString());
            }
        }

    }
}