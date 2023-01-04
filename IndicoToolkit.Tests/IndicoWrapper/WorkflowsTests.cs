using Xunit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using IndicoV2.Submissions.Models;

using IndicoToolkit.Types;
using IndicoToolkit.IndicoWrapper;

namespace IndicoToolkit.Tests
{
    public class WorkflowsFixture : IDisposable
    {
        public Workflows Wflow { get; private set; }
        public List<int> SubmissionIds {get; set; }

        public WorkflowsFixture()
        {
            Wflow = new Workflows(Utils.client);
            SubmissionIds = new List<int>();
        }

        public void Dispose()
        {
            // ... clean up test data if needed
        }

    }
    public class WorkflowsTests : IClassFixture<WorkflowsFixture>
    {
        WorkflowsFixture Fixture { get; }

        public WorkflowsTests(WorkflowsFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async void SubmitToWorkflowAndRetrieveOnDoc_ValidWorkflow_ShouldSubmitAndRetrieve()
        {
            IEnumerable<int> submissionIds = await Fixture.Wflow.SubmitDocumentsToWorkflow(Utils.workflowId, Utils.filePaths);
            Fixture.SubmissionIds = submissionIds.ToList();
            await Fixture.Wflow.WaitForSubmissionToProcess(Fixture.SubmissionIds[0]);
            ISubmission submission = await Fixture.Wflow.GetSubmissionObject(Fixture.SubmissionIds[0]);
            var resultOutput = await Fixture.Wflow.GetStorageObject(submission.ResultFile);
            using (var reader = new JsonTextReader(new StreamReader(resultOutput)))
            {
                dynamic json = JsonSerializer.Create().Deserialize(reader);
                string etlOutput = (json.etl_output);
                OnDocOCR onDocObject = await Fixture.Wflow.GetOnDocOCRFromEtlUrl(etlOutput);
                Assert.Equal(onDocObject.pages.Count, 2);
                Assert.Equal(onDocObject.num_pages, 2);
            }
        }

        [Fact]
        public async void MarkSubmissionAsRetrieved_ShouldRetrieve()
        {
            ISubmission sub = await Fixture.Wflow.MarkSubmissionAsRetrieved(Fixture.SubmissionIds[0]);
            Assert.Equal(sub.Status, SubmissionStatus.COMPLETE);
        }

        [Fact]
        public async void GetCompleteSubmissionObject_ShouldRetrieveMultiple()
        {
            IEnumerable<ISubmission> submissions = await Fixture.Wflow.GetCompleteSubmissionObjects(Utils.workflowId);
            Assert.True(submissions.Count() > 1);
        }

        [Fact]
        public async void GetSubmissionObject_ShouldGet()
        {
            var submission = await Fixture.Wflow.GetSubmissionObject(Fixture.SubmissionIds[0]);
            Assert.True(submission != null);
            Assert.True(submission is ISubmission);
        }

        [Fact]
        public async void GetSubmissionResultsFromIds_NotRawJSON_ShouldGetMultiple()
        {
            List<dynamic> submissions = await Fixture.Wflow.GetSubmissionResultsFromIds(Fixture.SubmissionIds);
            Assert.True(submissions.Count == Utils.filePaths.Count);
            foreach (var submission in submissions)
            {
                Assert.True(submission is WorkflowResult);
            }
        }

        [Fact]
        public async void GetSubmissionResultsFromIds_RawJSON_ShouldGetMultiple()
        {
            List<dynamic> submissions = await Fixture.Wflow.GetSubmissionResultsFromIds(Fixture.SubmissionIds, returnRawJson: true);
            Assert.True(submissions.Count == Utils.filePaths.Count);
            foreach (var submission in submissions)
            {
                Assert.False(submission is WorkflowResult);
            }
        }
    }
}