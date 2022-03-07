using Xunit;
using IndicoToolkit;
using IndicoToolkit.Types;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;

namespace IndicoToolkit.Tests
{
    public class PredictionTests 
    {
        [Fact]
        public void TestGetValue()
        {
            JObject val = JObject.Parse(@"{
                'Key': 'Value',
            }");
            Prediction prediction = new Prediction(val);
            Assert.Equal(prediction.getValue("Key"), val["Key"]);
        }

        [Fact]
        public void TestRemoveKey()
        {
            JObject val = JObject.Parse(@"{
                'Key': 'Value',
            }");
            Prediction prediction = new Prediction(val);
            prediction.removeKey("Key");
            Assert.Equal(prediction.getValue("Key"), null);
        }
    }
}