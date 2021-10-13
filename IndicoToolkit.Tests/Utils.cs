using Newtonsoft.Json;
using System;
using System.IO;
using Indico;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace IndicoToolkit.Tests
{
    public class Utils 
    {
        static public string file_dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        static public string host = Environment.GetEnvironmentVariable("INDICO_HOST");
        static public string api_key = Environment.GetEnvironmentVariable("INDICO_KEY");                
        static public IndicoConfig config = new IndicoConfig(host: host, apiToken: api_key);
        static public IndicoClient client = new IndicoClient(config);
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
    }
}