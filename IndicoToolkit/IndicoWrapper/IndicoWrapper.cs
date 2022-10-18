using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using IndicoV2;
using IndicoV2.Storage.Models;
using IndicoV2.Jobs.Models;

using IndicoToolkit.Types;
using IndicoToolkit.Exception;

namespace IndicoToolkit.IndicoWrapper
{
    /// <summary>
    /// Class <c>IndicoWrapper</c> supports shared API functionality
    /// </summary>
    public class IndicoWrapper
    {
        public IndicoClient client;
        public IndicoWrapper(IndicoClient _client)
        {
            this.client = _client;
        }

        /// <summary>
        /// Retrieves storage object with given storage url
        /// </summary>
        /// <param name="storageUrl">Url of corresponding storage object</param>
        public async Task<Stream> GetStorageObject(string storageUrl)
        {
            Uri storageUri = new Uri(storageUrl);
            return await client.Storage().GetAsync(storageUri, default);
        }

        /// <summary>
        /// Uploads files and returns newly created storage urls
        /// </summary>
        /// <param name="filePaths">List of local file paths to upload</param>
        /// <returns>List of storage urls to the corresponding file paths</returns>
        public async Task<IEnumerable<string>> CreateStorageUrls(List<string> filePaths)
        {
            List<string> storageUrls = new List<string>();
            IEnumerable<IFileMetadata> uploads = await client.Storage().UploadAsync(filePaths, default);
            foreach (IFileMetadata uploadMetadata in uploads)
            {
                storageUrls.Add(uploadMetadata.Path);
            }
            return storageUrls;
        }

        /// <summary>
        /// Retrieves job status with given job id
        /// </summary>
        /// <param name="jobId">Id of corresponding job</param>
        /// <returns></returns>
        public async Task<JobStatus> GetJobStatus(string jobId)
        {
            return await client.Jobs().GetStatusAsync(jobId);
        }

        /// <summary>
        /// Makes raw graphQL request with given query, operationName, and variables
        /// </summary>
        /// <param name="query">graphQL query to enact</param>
        /// <param name="operationName">name of the operation</param>
        /// <param name="variables">variables being inputted into request</param>
        /// <returns></returns>
        public async Task<JObject> GraphQLRequest(string query, string operationName, dynamic variables)
        {
            return await client.GraphQLRequest().Call(query, operationName, variables);
        }

        /// <summary>
        /// Submit samples directly to a model. Note: documents must already by in raw text
        /// </summary>
        /// <param name="modelId">The model Id to submit to</param>
        /// <param name="samples">A list containing the text samples you want to submit</param>
        /// <returns>A list of Predictions</returns>
        public async Task<List<Prediction>> GetPredictionsWithModelId(int modelId, List<string> samples)
        {
            List<Prediction> predictions = new List<Prediction>();
            string jobId = await client.Models().Predict(modelId, samples, default);
            JObject jobResult = await client.Jobs().GetResultAsync<JObject>(jobId);
            JobStatus status = await GetJobStatus(jobId);
            if (status != JobStatus.SUCCESS)
            {
                throw new ToolkitStatusException(
                    $"Predictions Failed, {status}"
                );
            }
            foreach (JObject predictionObject in jobResult["result"])
            {
                Prediction prediction = predictionObject.ToObject<Prediction>();
                predictions.Add(prediction);
            }
            return predictions;
        }
    }

}