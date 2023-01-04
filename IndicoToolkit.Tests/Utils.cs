using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using IndicoV2;
using Newtonsoft.Json.Linq;
using IndicoToolkit.Types;

namespace IndicoToolkit.Tests
{
    public class Utils
    {
        static public string file_dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        static public string host = "https://app.indico.io";
        static public string api_key = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpZCI6MTg1MTUzLCJ1c2VyX2lkIjoxNDc3OSwidXNlcl9lbWFpbCI6Im5hdGUuc2hpbUBpbmRpY28uaW8iLCJpYXQiOjE2NDk3MDQ1NDYsImF1ZCI6WyJpbmRpY286cmVmcmVzaF90b2tlbiJdfQ.XnAH_F0xcaMmSv5LwLhUTfcoqdZ1iT4ovb2daFNRqy0";
        static public IndicoClient client = new Client(host: host, apiTokenString: api_key).Create();
        static public int datasetId = 11642;
        static public int workflowId = 3965;
        static public int modelId = 38871;
        static public List<string> filePaths = new List<string>()
        {
            Path.Join(Utils.file_dir, "data/simple_doc.pdf"), 
            Path.Join(Utils.file_dir, "data/samples/fin_disc.pdf")
        };
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
            if (confidence is null)
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

        public static Position createPosition(
            int top = 1,
            int bottom = 10,
            int left = 5,
            int right = 15,
            float bbTop = 1f,
            float bbBot = 10f,
            float bbLeft = 5f,
            float bbRight = 15f,
            int pageNum = 0
        )
        {
            return new Position(
                top,
                bottom,
                left,
                right,
                bbTop,
                bbBot,
                bbLeft,
                bbRight,
                pageNum
            );
        }
    }
}