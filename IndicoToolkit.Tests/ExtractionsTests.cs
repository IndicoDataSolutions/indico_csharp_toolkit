using Xunit;
using IndicoToolkit;
using IndicoToolkit.Types;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

namespace IndicoToolkit.Tests
{
    public class ExtractionsTests 
    {
        static string jsonPath = Path.Join(Utils.file_dir, "data/samples/fin_disc_result.json");

        [Fact]
        public void TestInit()
        {
            List<string> fpaths = new List<string>{jsonPath};
            Console.WriteLine(fpaths);
            List<Prediction> staticExtractPreds = new List<Prediction>() {
                new Prediction(JObject.Parse(@"{
                'Key': 'Value',
                }")),
            };
            Extractions extractions = new Extractions(staticExtractPreds);
            Assert.Equal(extractions.Preds(), staticExtractPreds);
            
        }

    }
}