using Newtonsoft.Json.Linq;

namespace IndicoToolkit.Types
{
    public class Prediction
    {
        private readonly JObject predictionValue;

        public JObject PredictionValue { get { return predictionValue; } }

        public Prediction(JObject _predictionValue)
        {
            this.predictionValue = _predictionValue;
        }

        public dynamic getValue(string key)
        {
            return predictionValue.SelectToken(key);
        }

        public void setValue(string key, JToken newValue)
        {
            predictionValue[key] = newValue;
        }

        public string getLabel()
        {
            return (string) predictionValue["label"];
        }

        public void removeKey(string key)
        {
            predictionValue.Remove(key);
        }
    }

    
}