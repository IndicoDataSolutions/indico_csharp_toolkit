using Xunit;
using IndicoToolkit.Types;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace IndicoToolkit.Tests
{
    public class WorkflowResultTests
    {

        [Fact]
        public void GetPredictions_Default_ShouldGet()
        {
            JObject result = Utils.LoadJson("data/samples/fin_disc_result.json");
            WorkflowResult workflowResult = new WorkflowResult(result, "Toolkit Test Financial Model");
            Console.WriteLine(workflowResult.DocumentResults);
            List<Prediction> predictions = workflowResult.GetPredictions();
            Assert.Equal(predictions.Count, 25);
        }

    }
}