using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using IndicoV2;
using IndicoV2.Ocr.Models;

using IndicoToolkit.Types;

// TODO: Add exception handling, Add standard preset_config object

namespace IndicoToolkit
{
    public class DocExtraction
    {
        public IndicoClient client;
        public DocumentExtractionPreset docExtractionPreset = DocumentExtractionPreset.OnDocument;
        public DocExtraction(IndicoClient _client)
        {
            this.client = _client;
        }

        public async Task<List<OnDoc>> RunOCR(List<string> filepaths)
        {
            List<OnDoc> results = new List<OnDoc>();

            foreach (string path in filepaths)
            {
                string jobID = await client.Ocr().ExtractDocumentAsync(path, docExtractionPreset);
                JObject result = await client.Jobs().GetResultAsync<JObject>(jobID);
                Uri resultUri = new Uri((string)result["url"]);
                var storageResult = await client.Storage().GetAsync(resultUri, default);
                using (var reader = new JsonTextReader(new StreamReader(storageResult)))
                {
                    List<OnDocPage> pages = JsonSerializer.Create().Deserialize<List<OnDocPage>>(reader);
                    OnDoc ondoc = new OnDoc(pages);
                    results.Add(ondoc);
                }
            }
            return results;
        }
    }
}