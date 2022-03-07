using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Indico;
using Indico.Jobs;
using System.Threading.Tasks;

using IndicoToolkit.Types;

namespace IndicoToolkit
{
    /// <summary>
    /// Class <c>Extractions</c> provides functionality for common extraction prediction use cases.
    /// </summary>
    public class Extractions {
        internal List<Prediction> preds = new List<Prediction>();
        public List<Prediction> removed_predictions = new List<Prediction>();
        
        public Extractions(List<Prediction> predictions)
        {
            this.preds = predictions;
        }

        public List<Prediction> Preds() 
        {
            return preds;
        }

        /// <summary>
        /// Method <c>toDictByLabel</c> generates a dictionary where key is label string and value is list of all predictions of that label.
        /// </summary>
        public Dictionary<string, List<Prediction>> toDictByLabel()
        {
            Dictionary<string, List<Prediction>> prediction_label_map = new Dictionary<string, List<Prediction>>();
            for (int i = 0; i < preds.Count; i++) 
            {
                Prediction pred = preds[i];
                string pred_label = pred.getValue("label");

                if (prediction_label_map.ContainsKey(pred_label)) {
                    prediction_label_map[pred_label].Append(pred);
                } else {
                    prediction_label_map.Add(pred_label, new List<Prediction>() {pred});
                }
            }
            return prediction_label_map;
        }
        
        /// <summary>
        /// Method <c>numPredictions</c> returns number of predictions.
        /// </summary>
        public int numPredictions() 
        {
            return preds.Count; 
        }

        /// <summary>
        /// Method <c>removeByConfidence</c> removes predictions that are less than given confidence.
        /// </summary>
        public void removeByConfidence(float confidence = 0.95f, List<string> labels = null) 
        {
            List<Prediction> high_conf_preds = new List<Prediction>();
            for (int i = 0; i < preds.Count; i++) {
                Prediction pred = preds[i];
                string pred_label = pred.getValue("label");
                if (labels != null && !labels.Contains(pred_label)) {
                    high_conf_preds.Append(pred);
                } else if (pred.getValue("confidence")[pred_label] >= confidence) {
                    high_conf_preds.Append(pred);
                } else {
                    removed_predictions.Append(pred);
                }
            }
            preds = high_conf_preds;
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
        public void removeKeys(List<string> keys_to_remove = default(List<string>))
        {
            if (keys_to_remove == null) {
                keys_to_remove = new List<string>() {"start", "end"};
            }
            for (int i = 0; i < preds.Count; i++) {
                Prediction pred = preds[i];
                for (int j = 0; j < keys_to_remove.Count; j++) {
                    string key = keys_to_remove[j];
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
            for (int i = 0; i < preds.Count; i++) {
                Prediction pred = preds[i];
                if (labels.Contains(pred.getValue("label"))) {
                    removed_predictions.Append(pred);
                } else {
                    newPreds.Append(pred);
                }
            }
            preds = newPreds;
        }

        /// <summary>
        /// Method <c>removeHumanAddedPredictions</c> removes predictions that were not added by the model (i.e. added by scripted or human review).
        /// </summary>
        public void removeHumanAddedPredictions()
        {
            List<Prediction> newPreds = new List<Prediction>();
            for (int i = 0; i < preds.Count; i++) {
                Prediction pred = preds[i];
                if (!isManuallyAddedPrediction(pred)) {
                    newPreds.Append(pred);
                }
            }
            preds = newPreds;
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