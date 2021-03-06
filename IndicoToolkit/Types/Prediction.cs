using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace IndicoToolkit.Types
{
    public class Prediction
    {
        internal JObject prediction;

        public Prediction(JObject val)
        {
            this.prediction = val;
        }

        public dynamic getValue(string key)
        {
            return prediction[key];
        }

        public void removeKey(string key)
        {
            prediction.Remove(key);
        }
    }

    
}