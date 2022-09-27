using Newtonsoft.Json;
using System;
using System.IO;
using IndicoV2;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using IndicoToolkit.Types;

namespace IndicoToolkit.Tests
{
    public class Utils
    {
        static public string file_dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        static public string host = "http://app.indico.io";
        static public string api_key = Environment.GetEnvironmentVariable("INDICO_KEY");
        static public IndicoClient client = new Client(host: host, apiTokenString: api_key).Create();
        public static dynamic LoadJson(string path)
        {
            string file_path = Path.Join(file_dir, path);
            using (StreamReader r = new StreamReader(file_path))
            {
                string json = r.ReadToEnd();
                dynamic obj = JsonConvert.DeserializeObject(json);
                return obj;
            }
        }

        public static JObject CreateNewConfidence(
            string label,
            float newConfidence
        )
        {
            return new JObject {
                { label, newConfidence}
            };
        }

        public static dynamic CreatePrediction(
            int start = 0,
            int end = 10,
            string label = "testLabel",
            string text = "text",
            JObject confidence = null
        )
        {
            JObject val = new JObject();
            val.Add("start", start);
            val.Add("end", end);
            val.Add("label", label);
            val.Add("text", text);
            if (confidence == null)
            {
                JObject newConfidence = new JObject{
                    {"testLabel", .9f}
                };
                val.Add("confidence", newConfidence);
            }
            else
            {
                val.Add("confidence", confidence);
            }
            Prediction prediction = new Prediction(val);
            return prediction;
        }
    }
}