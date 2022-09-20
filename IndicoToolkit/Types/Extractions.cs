using System.Linq;
using System.Collections.Generic;

using IndicoToolkit.Types;

namespace IndicoToolkit.Types
{
    /// <summary>
    /// Class <c>Extractions</c> provides functionality for common extraction prediction use cases.
    /// </summary>
    public class Extractions {
        public List<Prediction> Preds { get; private set; } 
        public List<Prediction> RemovedPreds { get; private set; }
        
        public Extractions(List<Prediction> predictions)
        {
            Preds = predictions;
            RemovedPreds = new List<Prediction>();
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
                string predLabel = pred.getValue("label");

                if (predictionLabelMap.ContainsKey(predLabel)) {
                    predictionLabelMap[predLabel].Append(pred);
                } else {
                    predictionLabelMap.Add(predLabel, new List<Prediction>() {pred});
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
        /// </summary>
        public void removeByConfidence(float confidence = 0.95f, List<string> labels = null) 
        {
            List<Prediction> highConfPreds = new List<Prediction>();
            for (int i = 0; i < Preds.Count; i++) {
                Prediction pred = Preds[i];
                string predLabel = pred.getValue("label");
                if (labels != null && !labels.Contains(predLabel)) {
                    highConfPreds.Append(pred);
                } else if (pred.getValue("confidence")[predLabel] >= confidence) {
                    highConfPreds.Append(pred);
                } else {
                    RemovedPreds.Append(pred);
                }
            }
            Preds = highConfPreds;
        }

        /// <summary>
        /// Method <c>removeExceptMaxConfidence</c> removes all predictions except the highest confidence within each specified class.
        /// </summary>
        public void removeExceptMaxConfidence(List<string> labels) 
        {
            /* TODO */
            return ;
        }
        
        /// <summary>
        /// Method <c>removeKeys</c> removes specified keys from prediction dictionaries.
        /// </summary>
        public void removeKeys(List<string> keysToRemove = default(List<string>))
        {
            if (keysToRemove == null) {
                keysToRemove = new List<string>() {"start", "end"};
            }
            for (int i = 0; i < Preds.Count; i++) {
                Prediction pred = Preds[i];
                for (int j = 0; j < keysToRemove.Count; j++) {
                    string key = keysToRemove[j];
                    pred.removeKey(key);
                }
            }
        }

        /// <summary>
        /// Method <c>removeAllByLabel</c> removes all predictions in list of labels.
        /// </summary>
        public void removeAllByLabel(List<string> labels)
        {
            List<Prediction> newPreds = new List<Prediction>();
            for (int i = 0; i < Preds.Count; i++) {
                Prediction pred = Preds[i];
                if (labels.Contains(pred.getValue("label"))) {
                    RemovedPreds.Append(pred);
                } else {
                    newPreds.Append(pred);
                }
            }
            Preds = newPreds;
        }

        /// <summary>
        /// Method <c>removeHumanAddedPredictions</c> removes predictions that were not added by the model (i.e. added by scripted or human review).
        /// </summary>
        public void removeHumanAddedPredictions()
        {
            List<Prediction> newPreds = new List<Prediction>();
            for (int i = 0; i < Preds.Count; i++) {
                Prediction pred = Preds[i];
                if (!isManuallyAddedPrediction(pred)) {
                    newPreds.Append(pred);
                }
            }
            Preds = newPreds;
        }

        /// <summary>
        /// Method <c>isManuallyAddedPrediction</c> checks if prediction has been manually added.
        /// </summary>
        public static bool isManuallyAddedPrediction(Prediction prediction)
        {
            if (prediction.getValue("start") is int && prediction.getValue("end") is int) {
                if (prediction.getValue("end") > prediction.getValue("start")) {
                    return false;
                }
            }
            return true;
        } 

        /// <summary>
        /// Method <c>selectMaxConfidence</c> gets the highest confidence prediction for a given field.
        /// </summary>
        internal Prediction selectMaxConfidence(string label) {
            /* TODO */
            return null;
        }

        /// <summary>
        /// Method <c>toCSV</c> writes three column CSV ('confidence', 'label', 'text')
        /// </summary>
        public void toCSV(string save_path, string filename = "", bool append_if_exists = true, bool include_start_end = false) {
            /* TODO */
            return ;
        }
        
    }
}