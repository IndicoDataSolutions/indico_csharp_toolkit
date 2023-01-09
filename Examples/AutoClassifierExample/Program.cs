using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using IndicoToolkit;
using IndicoToolkit.AutoClass;

namespace Examples
{
    /// <summary>
    /// Create a CSV that can be used to train a document classification model without any labeling 
    /// using an organized directory/folder structure. Each folder/directory should contain only one file 
    /// type.
    /// For example, you would target '/base_directory/' if you had your files organized like:
    /// /base_directory/
    /// /base_directory/invoices/ -> contains only invoice files
    /// /base_directory/disclosures/ -> contains only disclosure files    
    /// </summary>
    public class AutoClassifierExample
    {
        private static string GetHost() => Environment.GetEnvironmentVariable("INDICO_HOST");
        private static string GetToken() => Environment.GetEnvironmentVariable("INDICO_KEY");
        private static string GetBaseDirectory() => Path.Join(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "base_directory");
        private static string GetFilePath() => Path.Join(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "auto_classifier.csv");

        public static async Task Main()
        {
            IndicoClient client = new Client(host: GetHost(), apiTokenString: GetToken()).Create();
            
            AutoClassifier autoClassifier = new AutoClassifier(client, GetBaseDirectory());
            autoClassifier.setFileStructure(new List<string>(){"pdf", "tiff"});
            autoClassifier.setDocumentText(batchSize: 5);
            autoClassifier.toCSV(GetFilePath());
        }
    }
}
