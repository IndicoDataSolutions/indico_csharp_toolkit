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
        public List<string> FilePaths { get; private set; }
        public int DatasetId { get; private set; }
        public int WorkflowId { get; private set; }
        public int SubmissionId { get; private set; }


        public WorkflowsFixture()
        {
            Wflow = new Workflows(Utils.client);
            FilePaths = new List<string>()
            {
                Path.Join(Utils.file_dir, "data/simple_doc.pdf"),
                Path.Join(Utils.file_dir, "data/samples/fin_disc.pdf")
            };
            DatasetId = 10887;
            WorkflowId = 3965;
            SubmissionId = 153001;
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
        public async void GetWorkflow_ValidWorkflow_ShouldGet()
        {
            var workflow = await Fixture.Wflow.GetWorkflow(Fixture.DatasetId);
            Assert.True(workflow != null);
        }

        [Fact]
        public async void SubmitToWorkflowAndRetrieveOnDoc_ValidWorkflow_ShouldSubmitAndRetrieve()
        {
            IEnumerable<int> submissionIds = await Fixture.Wflow.SubmitDocumentsToWorkflow(Fixture.WorkflowId, Fixture.FilePaths);
            await Fixture.Wflow.WaitForSubmissionToProcess(submissionIds.ElementAt<int>(0));
            ISubmission submission = await Fixture.Wflow.GetSubmissionObject(submissionIds.ElementAt<int>(0));
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
            ISubmission sub = await Fixture.Wflow.MarkSubmissionAsRetrieved(Fixture.SubmissionId);
            Assert.Equal(sub.Status, SubmissionStatus.COMPLETE);
        }

        [Fact]
        public async void GetCompleteSubmissionObject_ShouldRetrieveMultiple()
        {
            IEnumerable<ISubmission> submissions = await Fixture.Wflow.GetCompleteSubmissionObjects(Fixture.WorkflowId);
            Assert.True(submissions.Count() > 1);
        }

        [Fact]
        public async void GetSubmissionObject_ShouldGet()
        {
            ISubmission submission = await Fixture.Wflow.GetSubmissionObject(Fixture.SubmissionId);
        }

        [Fact]
        public async void GetSubmissionResultsFromIds_NotRawJSON_ShouldGetMultiple()
        {
            IEnumerable<int> submissionIds = await Fixture.Wflow.SubmitDocumentsToWorkflow(Fixture.WorkflowId, Fixture.FilePaths);
            List<dynamic> submissions = await Fixture.Wflow.GetSubmissionResultsFromIds(submissionIds.ToList());
            Assert.True(submissions.Count == Fixture.FilePaths.Count);
            foreach (var submission in submissions)
            {
                Assert.True(submission is WorkflowResult);
            }
        }

        [Fact]
        public async void GetSubmissionResultsFromIds_RawJSON_ShouldGetMultiple()
        {
            IEnumerable<int> submissionIds = await Fixture.Wflow.SubmitDocumentsToWorkflow(Fixture.WorkflowId, Fixture.FilePaths);
            List<dynamic> submissions = await Fixture.Wflow.GetSubmissionResultsFromIds(submissionIds.ToList(), returnRawJson: true);
            Assert.True(submissions.Count == Fixture.FilePaths.Count);
            foreach (var submission in submissions)
            {
                Assert.False(submission is WorkflowResult);
            }
        }
    }
}