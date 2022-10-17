using Xunit;
using IndicoToolkit.Types;
using Newtonsoft.Json.Linq;

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
        public void TestSetValue()
        {
            JObject val = JObject.Parse(@"{
                'Key': 'Value',
            }");
            Prediction prediction = new Prediction(val);
            prediction.setValue("Key", (JToken) "NewValue");
            Assert.Equal((string) prediction.getValue("Key"), "NewValue");
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