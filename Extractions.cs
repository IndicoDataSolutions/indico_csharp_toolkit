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
        internal List<Dictionary<string, string>> preds = new List<Dictionary<string, string>>();
        public List<Dictionary<string, string>> removed_predictions = new List<Dictionary<string, string>>();
        
        /// def __init__(self, predictions: List[dict]):
        public Extractions(List<Dictionary<string, string>> predictions){
            this.preds = predictions;
        }

        /*
        @property
        def to_dict_by_label(self) -> Dict[str, list]:
        */
        /// <summary>
        /// Method <c>toDictByLabel</c> generates a dictionary where key is label string and value is list of all predictions of that label.
        /// </summary>
        public Dictionary<string, List<Dictionary<string, string>>> toDictByLabel() {
            /* TODO */
            return ;
        }

        /*
        @property
        def num_predictions(self) -> int:
        */
        /// <summary>
        /// Method <c>numPredictions</c> returns number of predictions.
        /// </summary>
        public int numPredictions() {
            return self.preds.Count;
        }

        /*
        @property
        def remove_by_confidence(self, confidence: float = 0.95, labels: List[str] = None):
        */
        /// <summary>
        /// Method <c>removeByConfidence</c> removes predictions that are less than given confidence.
        /// </summary>
        public void removeByConfidence(float confidence = 0.95f, List<string> labels = null) {
            /* TODO */
            return ;
        }

        
        
        /*
        @staticmethod
        def _set_confidence_key_to_max_value(preds):
        */
        internal static List<> setConfidenceKeyToMaxValue(List<> preds) {
            /* TODO */
            return ;
        }

        /*
        @staticmethod
        def get_label_set(predictions: List[dict]) -> Set[str]:
        */
        /// <summary>
        /// Method <c>getLabelSet</c> gets the included labels from a list of predictions.
        /// </summary>
        public static HashSet<string> getLabelSet(List<Dictionary<string, string>> predictions) {
            /* TODO */
            return ;
        }
        
        /*
        def __len__(self):
        */


        /*
        def to_csv(
            self,
            save_path: str,
            filename: str = "",
            append_if_exists: bool = True,
            include_start_end: bool = False,
        ) -> None
        */
        public void toCSV(string save_path, string filename = "", bool append_if_exists = true, bool include_start_end = false) {
            /* TODO */
            return ;
        }
        
    }
}