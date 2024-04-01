using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using IndicoToolkit;
using IndicoToolkit.IndicoWrapper;

using IndicoV2.Ocr.Models;

namespace Examples
{
    /// <summary>   
    /// Retrieves a list of raw full document texts for all files in a folder
    /// </summary>
    public class SubmittingToDocExtractionExample
    {
        private static string GetHost() => Environment.GetEnvironmentVariable("INDICO_HOST");
        private static string GetToken() => Environment.GetEnvironmentVariable("INDICO_KEY");
        private static string GetDirectory() => Environment.GetEnvironmentVariable("DIRECTORY_NAME");

        public static async Task Main()
        {
            /// Instantiate the DocExtraction class
            IndicoClient client = new Client(host: GetHost(), apiTokenString: GetToken()).Create();
            DocExtraction docExtraction = new DocExtraction(client, docExtractionPreset: DocumentExtractionPreset.OnDocument);

            /// Collect files to submit
            FileProcessing Fp = FileProcessing(); 
            Fp.GetFilePathsFromDir(Path.Join(Directory.GetCurrentDirectory(), GetDirectory()); 

            /// Submit documents with optional text setting and save results to variable
            List<OnDoc> docTexts = new List<OnDoc>();
            foreach (List<string> paths in Fp.BatchFiles(batchSize: 10))
            {
                docTexts.Add(docExtraction.RunOCR(paths));
            }
            Console.WriteLine(docTexts);
        }
    }
}
