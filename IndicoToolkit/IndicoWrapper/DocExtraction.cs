using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Indico;
using Indico.Jobs;
using System.Threading.Tasks;

using IndicoToolkit.Types;

// TODO: Add exception handling, Add standard preset_config object

namespace IndicoToolkit
{
    public class DocExtraction {
        public IndicoClient client;
        public JObject ocr_config = new JObject(){{"preset_config", "ondocument"}};
        public DocExtraction(IndicoClient _client){
            this.client = _client;
        }

        public async Task<List<OnDoc>> RunOCR(List<string> filepaths){
            List<OnDoc> results = new List<OnDoc>();
            var ocr = client.DocumentExtraction(this.ocr_config);
            foreach(string path in filepaths){
                Job job = await ocr.Exec(path);
                var result = await job.Result();
                string url = (string)result.GetValue("url");
                var blob = await client.RetrieveBlob(url).Exec();
                List<OnDocPage> pages = JsonConvert.DeserializeObject<List<OnDocPage>>(blob.AsString());
                OnDoc ondoc = new OnDoc(pages);
                results.Add(ondoc);
            }
            return results;
        }
    }
}