using Xunit;
using IndicoToolkit.Exception;
using IndicoToolkit.Types;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace IndicoToolkit.Tests
{
    public class WorkflowResultFixture : IDisposable
    {

        public JObject ResultWithReview { get; private set; }
        public JObject ResultWithoutReview { get; private set; }
        public WorkflowResult WorkflowResultObjectWithReview { get; private set; }
        public WorkflowResult WorkflowResultObjectWithoutReview { get; private set; }
        public WorkflowResultFixture()
        {
            ResultWithReview = Utils.LoadJson("data/samples/med_extractions.json");
            ResultWithoutReview = Utils.LoadJson("data/samples/fin_disc_result.json");
            WorkflowResultObjectWithReview = new WorkflowResult(ResultWithReview, "Medical Record Extraction");
            WorkflowResultObjectWithoutReview = new WorkflowResult(ResultWithoutReview, "Toolkit Test Financial Model");
        }

        public void Dispose()
        {
            // ... clean up test data if needed
        }

    }
    public class WorkflowResultTests : IClassFixture<WorkflowResultFixture>
    {

        WorkflowResultFixture Fixture { get; }

        public WorkflowResultTests(WorkflowResultFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void GetPredictions_NotUsingReview_ShouldGet()
        {
            List<Prediction> predictions = Fixture.WorkflowResultObjectWithoutReview.GetPredictions();
            Assert.Equal(predictions.Count, 25);
        }

        [Fact]
        public void GetPredictions_UsingReview_ShouldGet()
        {
            List<Prediction> predictions = Fixture.WorkflowResultObjectWithReview.GetPredictions();
            Assert.Equal(predictions.Count, 44);
        }

        [Fact]
        public void GetPredictions_InvalidModelName_ShouldThrowException()
        {
            WorkflowResult workflowResult = new WorkflowResult(Fixture.ResultWithReview, "Invalid Model Name");
            Assert.Throws<ToolkitInputException>(() => workflowResult.GetPredictions());
        }

        [Fact]
        public void VerifyWorkflowResultPropertiesWithoutReview()
        {
            WorkflowResult workflowResult = Fixture.WorkflowResultObjectWithoutReview;
            Assert.Equal(workflowResult.ModelName, "Toolkit Test Financial Model");
            Assert.Equal(workflowResult.EtlUrl, "indico-file:///storage/submission/1440/3bf675bc-ceaf-4def-990b-ce1eff958645/65f50deb-7be2-4f4c-9c39-756481cdaf6c_9c7a45d6-50f5-48c2-be44-350208fd38f5_etl_output.json");
            Assert.Equal(workflowResult.AvailableModelNames, new List<string>() { "Toolkit Test Financial Model" });
            Assert.Equal(workflowResult.SubmissionId, "c9913738-9c5b-11eb-9b92-4e543323b561");
            Assert.Equal(workflowResult.Errors, new List<string>());
            Assert.Equal(workflowResult.ReviewId, null);
            Assert.Equal(workflowResult.ReviewerId, null);
            Assert.Equal(workflowResult.ReviewNotes, null);
            Assert.Equal(workflowResult.ReviewRejected, null);
            Assert.Equal(workflowResult.AdminReview, null);
        }

        [Fact]
        public void VerifyWorkflowResultPropertiesWithReview()
        {
            WorkflowResult workflowResult = Fixture.WorkflowResultObjectWithReview;
            Assert.Equal(workflowResult.ModelName, "Medical Record Extraction");
            Assert.Equal(workflowResult.EtlUrl, "indico-file:///storage/submission/782/45453/33297/etl_output.json");
            Assert.Equal(workflowResult.AvailableModelNames, new List<string>() { "Medical Record Extraction" });
            Assert.Equal(workflowResult.SubmissionId, "45453");
            Assert.Equal(workflowResult.Errors, new List<string>() { "this is a test error" });
            Assert.Equal(workflowResult.ReviewId, "1");
            Assert.Equal(workflowResult.ReviewerId, null);
            Assert.Equal(workflowResult.ReviewNotes, null);
            Assert.Equal(workflowResult.ReviewRejected, null);
            Assert.Equal(workflowResult.AdminReview, null);
        }
    }
}