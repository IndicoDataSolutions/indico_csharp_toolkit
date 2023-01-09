using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using IndicoToolkit;
using IndicoToolkit.IndicoWrapper;
using IndicoToolkit.AutoReview;
using IndicoToolkit.Types;

namespace Examples
{
    /// <summary>
    /// Submit documents to a workflow, auto review them and submit them for human review
    /// </summary>
    public class AutoReviewPredictionsExample
    {
        private static string GetHost() => Environment.GetEnvironmentVariable("INDICO_HOST");
        private static string GetToken() => Environment.GetEnvironmentVariable("INDICO_KEY");
        private static string GetWorkflowId() => Environment.GetEnvironmentVariable("WORKFLOW_ID");
        private static string GetModelName() => Environment.GetEnvironmentVariable("MODEL_NAME");
        
        public static async Task Main()
        {
            /// Instantiate the Workflows class
            IndicoClient client = new Client(host: GetHost(), apiTokenString: GetToken()).Create();
            Workflows Wflow = new Workflows(client);

            /// Submit a document and get predictions
            IEnumerable<int> submissionIds = await Wflow.SubmitDocumentsToWorkflow(GetWorkflowId(), new List<string>(){"workflow-sample.pdf"});
            List<WorkflowResult> results = await Wflow.GetSubmissionResultsFromIds(submissionIds.ToList());
            List<Prediction> predictions = results[0].GetPredictions();

            /// Set up reviewer and review predictions
            List<FunctionConfig> fieldConfig = new List<FunctionConfig>()
            {
                new FunctionConfig
                (
                    "rejectByConfidence",
                    new Kwargs
                    (
                        labels: new List<string>() { "Name" },
                        threshold: .99f
                    )
                ),
            };
            ReviewConfiguration reviewConfig = new ReviewConfiguration(fieldConfig);
            AutoReviewer autoReviewer = new AutoReviewer(predictions, reviewConfig);
            autoReviewer.applyReviews();

            /// Submit review
            string query = @"
                mutation SubmitReview($changes: JSONString, $rejected: Boolean, $submissionId: Int!, $forceComplete: Boolean) {
                    submitAutoReview(changes: $changes, rejected: $rejected, submissionId: $submissionId, forceComplete: $forceComplete) {
                        jobId
                    }
                }
            ";
            string operationName = "SubmitReview";
            var updatedPredsAsJSON = JsonConvert.SerializeObject(autoReviewer.UpdatedPredictions());
            dynamic variables = new { changes = updatedPredsAsJSON, rejected = false, submissionId = submissionId, forceComplete = false };
            return await client.GraphQLRequest().Call(query, operationName, variables);
        }
    }
}
