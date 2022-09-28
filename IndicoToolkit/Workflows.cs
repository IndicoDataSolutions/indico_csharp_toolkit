using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using IndicoV2;
using IndicoV2.Workflows.Models;
using IndicoV2.Submissions.Models;

using IndicoToolkit.Types;

namespace IndicoToolkit
{
    /// <summary>
    /// Class <c>Workflows</c> supports Workflow-related API calls
    /// </summary>
    public class Workflows
    {
        public IndicoClient client;
        public Workflows(IndicoClient _client)
        {
            this.client = _client;
        }

        /// <summary>
        /// Retrieves workflow with given datasetID
        /// </summary>
        /// <param name="datasetId">ID of corresponding dataset</param>
        public async Task<IEnumerable<IWorkflow>> GetWorkflow(int datasetId)
        {
            return await client.Workflows().ListAsync(datasetId, default);
        }

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
        public async Task<List<OnDoc>> GetOnDocOCRFromEtlUrl(string etlUrl)
        {
            List<OnDoc> results = new List<OnDoc>();
            Uri resultUri = new Uri(etlUrl);
            var storageResult = await client.Storage().GetAsync(resultUri, default);
            using (var reader = new JsonTextReader(new StreamReader(storageResult)))
            {
                List<OnDocPage> pages = JsonSerializer.Create().Deserialize<List<OnDocPage>>(reader);
                OnDoc ondoc = new OnDoc(pages);
                results.Add(ondoc);
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

    }

}