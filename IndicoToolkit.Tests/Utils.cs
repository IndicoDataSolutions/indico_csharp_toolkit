using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace IndicoToolkit.Tests
{
    public class Utils 
    {
        static public string file_dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        
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