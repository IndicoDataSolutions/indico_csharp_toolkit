using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using IndicoToolkit;
using IndicoToolkit.IndicoWrapper;

namespace Examples
{
    public class SubmittingToWorkflowExample
    {
        private static string GetHost() => Environment.GetEnvironmentVariable("INDICO_HOST");
        private static string GetToken() => Environment.GetEnvironmentVariable("INDICO_KEY");
        private static string GetWorkflowId() => Environment.GetEnvironmentVariable("WORKFLOW_ID");

        public static async Task Main()
        {
            /// Instantiate the Workflows class
            IndicoClient client = new Client(host: GetHost(), apiTokenString: GetToken()).Create();
            Workflows Wflow = new Workflows(client);

            /// Get workflow and submit documents
            IEnumerable<int> submissionIds = await Wflow.SubmitDocumentsToWorkflow(GetWorkflowId(), new List<string>(){"workflow-sample.pdf"});

            /// Submit documents, await the results and read results.
            foreach (int submissionId in submissionIds.ToList())
            {
                await Wflow.WaitForSubmissionToProcess(submissionId);
                ISubmission submission = await Wflow.GetSubmissionObject(submissionId);
                var resultOutput = await Wflow.GetStorageObject(submission.ResultFile);
                Console.ReadLine();
            }
        }
    }
}
