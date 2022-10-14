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
        static public string host = Environment.GetEnvironmentVariable("INDICO_HOST");
        static public string api_key = Environment.GetEnvironmentVariable("INDICO_KEY");
        static public IndicoClient client = new Client(host: "http://app.indico.io", apiTokenString: api_key).Create();
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

        public static List<T> LoadJsonIntoObject<T>(string path)
        {
            List<T> typeObjects = new List<T>();
            string file_path = Path.Join(file_dir, path);
            using (StreamReader r = new StreamReader(file_path))
            {
                string json = r.ReadToEnd();
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                dynamic objs = JsonConvert.DeserializeObject(json, settings);
                foreach (var obj in objs)
                {
                    JObject objAsJObject = obj as JObject;
                    T typeObject = objAsJObject.ToObject<T>();
                    typeObjects.Add(typeObject);
                }
                return typeObjects;
            }
        }

        public static Dictionary<string, float> CreateNewConfidence(
            string label,
            float newConfidence
        )
        {
            return new Dictionary<string, float> {
                { label, newConfidence}
            };
        }

        public static dynamic CreatePrediction(
            int start = 0,
            int end = 10,
            int pageNum = 0,
            string label = "testLabel",
            string text = "text",
            Dictionary<string, float> confidence = null,
            Grouping[] groupings = null,
            bool rejected = false,
            bool accepted = false
        )
        {
            Prediction prediction = new Prediction(
                start,
                end,
                pageNum,
                label,
                text,
                confidence,
                groupings,
                rejected,
                accepted
            );
            Dictionary<string, float> defaultConfidence = new Dictionary<string, float> {
                {"testLabel", .9f}
            };
            Grouping[] defaultGroupings = new Grouping[] { new Grouping("testGroupName", 0, "testGroupId") };

            prediction.Confidence = confidence is null ? defaultConfidence : confidence;
            prediction.Groupings = groupings is null ? defaultGroupings : groupings;
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