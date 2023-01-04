using Xunit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using IndicoV2;
using IndicoV2.Submissions.Models;
using IndicoV2.Jobs.Models;
using IndicoV2.Ocr.Models;

using IndicoToolkit.Types;
using IndicoToolkit.IndicoWrapper;

namespace IndicoToolkit.Tests
{
    public class IndicoClientWrapperFixture : IDisposable
    {
        public IndicoClientWrapper indicoWrapper { get; private set; }

        public IndicoClientWrapperFixture()
        {
            indicoWrapper = new IndicoClientWrapper(Utils.client);
        }

        public void Dispose()
        {
            // ... clean up test data if needed
        }

    }
    public class IndicoClientWrapperTests : IClassFixture<IndicoClientWrapperFixture>
    {
        IndicoClientWrapperFixture Fixture { get; }

        public IndicoClientWrapperTests(IndicoClientWrapperFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async void GetStorageObject_ShouldRetrieve()
        {
            Workflows workflow = new Workflows(Utils.client);
            IEnumerable<int> submissionIds = await workflow.SubmitDocumentsToWorkflow(Utils.workflowId, Utils.filePaths);
            List<int> submissionIdsList = submissionIds.ToList();
            await workflow.WaitForSubmissionToProcess(submissionIdsList[0]);
            ISubmission job = await Fixture.indicoWrapper.client.Submissions().GetAsync(submissionIdsList[0]);
            var storageResult = await Fixture.indicoWrapper.GetStorageObject(job.ResultFile);
            Assert.True(storageResult != null);
        }

        [Fact]
        public async void CreateStorageUrls_ShouldCreate()
        {
            IEnumerable<string> storageURLs = await Fixture.indicoWrapper.CreateStorageUrls(Utils.filePaths);
            Assert.Equal(storageURLs.ToList().Count, 2);
        }

        [Fact]
        public async void GetJobStatus_ShouldGet()
        {
            DocumentExtractionPreset docExtractionPreset = DocumentExtractionPreset.OnDocument;
            string jobId = await Fixture.indicoWrapper.client.Ocr().ExtractDocumentAsync(Utils.filePaths[0], docExtractionPreset);
            JobStatus status = await Fixture.indicoWrapper.GetJobStatus(jobId);
            Assert.Equal(status, JobStatus.PENDING);
            await Fixture.indicoWrapper.client.Jobs().GetResultAsync<dynamic>(jobId);
            status = await Fixture.indicoWrapper.GetJobStatus(jobId);
            Assert.Equal(status, JobStatus.SUCCESS);
        }

        [Fact]
        public async void GraphQLRequest_GetDatasets_ShouldRequest()
        {
            string query = @"
            query ListDatasets($limit: Int){
                datasetsPage(limit: $limit) {
                    datasets {
                        id
                        name
                        rowCount
                    }
                }
            }
            ";
            string operationName = "ListDatasets";
            dynamic variables = new { limit = 1 };
            JObject result = await Fixture.indicoWrapper.GraphQLRequest(query, operationName, variables);
            Assert.True(result != null);
        }

        [Fact]
        public async void GetPredictionsWithModelId_ValidModelId_ShouldGet()
        {
            DocExtraction docExtraction = new DocExtraction(Utils.client);
            List<OnDoc> onDocResults = await docExtraction.RunOCR(Utils.filePaths);
            List<string> rawTexts = new List<string>();
            foreach (OnDoc onDoc in onDocResults)
            {
                rawTexts.Add(onDoc.GetFullText());
            }
            List<Prediction> predictions = await Fixture.indicoWrapper.GetPredictionsWithModelId(Utils.modelId, rawTexts);
            Assert.Equal(predictions.Count, 11);
            foreach (var pred in predictions)
            {
                Assert.True(pred is Prediction);
            }
        }

        [Fact]
        public async void GetPredictionsWithModelId_ValidModelIdDummyRawText_ShouldGetNothing()
        {
            List<string> rawTexts = new List<string>()
            {
                "This is some raw text",
                "This is some more raw text"
            };
            List<Prediction> predictions = await Fixture.indicoWrapper.GetPredictionsWithModelId(Utils.modelId, rawTexts);
            Assert.Equal(predictions.Count, 0);
        }
    }
}