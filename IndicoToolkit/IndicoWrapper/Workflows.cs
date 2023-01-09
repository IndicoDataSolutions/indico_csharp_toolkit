using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IndicoV2;
using IndicoV2.Workflows.Models;
using IndicoV2.Submissions.Models;

using IndicoToolkit.Types;
using IndicoToolkit.Exception;

namespace IndicoToolkit.IndicoWrapper
{
    /// <summary>
    /// Class <c>Workflows</c> supports Workflow-related API calls
    /// </summary>
    public class Workflows : IndicoClientWrapper
    {
        public Workflows(IndicoClient _client) : base(_client) { }

        /// <summary>
        /// Submit documents to workflow
        /// </summary>
        /// <param name="workflowId">Workflow to submit to</param>
        /// <param name="pdfFilePaths">List of paths to local documents you would like to submit</param>
        /// <returns>List of submission Ids</returns>
        public async Task<IEnumerable<int>> SubmitDocumentsToWorkflow(int workflowId, IEnumerable<string> pdfFilePaths)
        {
            return await client.Submissions().CreateAsync(workflowId, pdfFilePaths);
        }

        /// <summary>
        /// Get ondocument OCR object from workflow result etl output
        /// </summary>
        /// <param name="etlUrl">url from "etl_output" key of workflow result json</param>
        /// <returns>'ondocument' OCR object</returns>
        public async Task<OnDocOCR> GetOnDocOCRFromEtlUrl(string etlUrl)
        {
            OnDocOCR results = new OnDocOCR();
            Uri resultUri = new Uri(etlUrl);
            var storageResult = await client.Storage().GetAsync(resultUri, default);
            using (var reader = new JsonTextReader(new StreamReader(storageResult)))
            {
                OnDocOCR onDocOCR = JsonSerializer.Create().Deserialize<OnDocOCR>(reader);
                // OnDoc ondoc = new OnDoc(pages);
                results = onDocOCR;
            }
            return results;
        }

        /// GetFileBytes()
        /// GetImgBytesFromEtlUrl

        /// <summary>
        /// Mark submission as retrieved
        /// </summary>
        /// <param name="submissionId">Submission to mark as retrieved</param>
        /// <returns>Submission object</returns>
        public async Task<ISubmission> MarkSubmissionAsRetrieved(int submissionId)
        {
            return await client.Submissions().MarkSubmissionAsRetrieved(submissionId);
        }

        /// <summary>
        /// Get all complete submission objects from given workflow
        /// </summary>
        /// <param name="workflowId">Workflow to get submissions from</param>
        /// <param name="submissionIds">Submissions to filter through and retrieve</param>
        /// <returns>List of complete submission objects in given workflow</returns>
        public async Task<IEnumerable<ISubmission>> GetCompleteSubmissionObjects(int workflowId, IEnumerable<int> submissionIds = default)
        {
            SubmissionFilter completeFilter = new SubmissionFilter();
            completeFilter.Status = SubmissionStatus.COMPLETE;
            return await client.Submissions().ListAsync(submissionIds, new List<int>() { workflowId }, completeFilter);
        }

        /// <summary>
        /// Get submission with given Id
        /// </summary>
        /// <param name="submissionId">Submission Id to query for</param>
        /// <returns>Submission object</returns>
        public async Task<ISubmission> GetSubmissionObject(int submissionId)
        {
            return await client.Submissions().GetAsync(submissionId);
        }

        /// <summary>
        /// Wait for submission to pass through workflow models and get result. If Review is enabled,
        /// result may be retrieved prior to human review.
        /// </summary>
        /// <param name="submissionIds">Ids of submission predictions to retrieve</param>
        /// <param name="timeout">seconds permitted for each submission prior to timing out</param>
        /// <param name="returnRawJson">If true return raw json result, otherwise return WorkflowResult object</param>
        /// <param name="throwExceptionForFailed">if true, ToolkitStatusError raised for failed submissions</param>
        /// <param name="returnFailedResults">if true, return objects for failed submissions</param>
        /// <param name="ignoreDeletedSubmissions">if true, ignore deleted submissions</param>
        /// <returns>workflow result objects</returns>
        public async Task<List<WorkflowResult>> GetSubmissionResultsFromIds(
            List<int> submissionIds,
            int timeout = 180,
            bool throwExceptionForFailed = false,
            bool returnFailedResults = true,
            bool ignoreDeletedSubmissions = false
        )
        {
            List<WorkflowResult> results = new List<WorkflowResult>();
            foreach (int subId in submissionIds)
            {
                await WaitForSubmissionToProcess(subId);
                ISubmission submission = await GetSubmissionObject(subId);
                if (submission.Status == SubmissionStatus.FAILED)
                {
                    string message = $"FAILURE, Submission: {subId}. {submission.Errors}";
                    if (throwExceptionForFailed)
                    {
                        throw new ToolkitStatusException(message);
                    }
                    else if (!returnFailedResults)
                    {
                        Console.WriteLine(message);
                        continue;
                    }
                }
                Stream stream = await CreateResult(submission.Id);
                var serializer = new JsonSerializer();
                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    dynamic result = serializer.Deserialize(jsonTextReader);
                    result.input_file = submission.InputFile;
                    {
                        results.Add(new WorkflowResult(result));
                    }
                }
            }
            return results;
        }

        /// <summary>
        ///  Returns result storage object from submissionId. 
        /// </summary>
        /// <param name="submissionId">Submission Id to get result from</param>
        /// <returns>Storage object as stream</returns>
        public async Task<Stream> CreateResult(int submissionId)
        {
            ISubmission job = await client.Submissions().GetAsync(submissionId);
            return await GetStorageObject(job.ResultFile);
        }

        /// <summary>
        /// Wait for submissions to reach a terminal status of "COMPLETE", "PENDING_AUTO_REVIEW",
        /// "FAILED", or "PENDING_REVIEW"
        /// </summary>
        /// <param name="submissionId">Id of corresponding submission</param>
        /// <returns></returns>
        public async Task WaitForSubmissionToProcess(int submissionId)
        {
            await client.GetSubmissionResultAwaiter().WaitReady(submissionId);
        }

    }

}