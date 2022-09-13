using System;
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

        public void setValue(string key, JToken value)
        {
            prediction[key] = value;
        }

        public string getLabel()
        {
            return (string) prediction["label"];
        }

        public void removeKey(string key)
        {
            prediction.Remove(key);
        }
    }

    
}