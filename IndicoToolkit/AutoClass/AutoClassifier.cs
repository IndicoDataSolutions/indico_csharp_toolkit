using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using IndicoV2;
using CsvHelper;

using IndicoToolkit.Types;
using IndicoToolkit.IndicoWrapper;


namespace IndicoToolkit.AutoClass
{
    public class AutoClassifier
    {
        public string directoryPath;
        public IndicoClient client;
        public List<string> filePaths = new List<string>();
        public List<string> fileClasses = new List<string>();
        public List<string> fileTexts = new List<string>();
        public List<string> acceptedTypes = new List<string>() { "*.pdf", "*.doc", "*.docx", "*.tiff", "*.tif", "*.png" };
        private FileProcessing fp = new FileProcessing();

        /// <summary>Class to create classification CSV based on file structure</summary>
        /// <param name="client">Instantiated IndicoClient.</param>
        /// <param name="directoryPath">Path to the base directory containing class subdirectories.</param>
        public AutoClassifier(IndicoClient client, string directoryPath)
        {
            this.client = client;
            this.directoryPath = directoryPath;
        }

        /// <summary>OCR filepaths and add full texts to this.FileTexts</summary>
        /// <param name="batchSize">Docs to OCR in parallel.</param>
        public async Task setDocumentText(int batchSize = 5)
        {
            DocExtraction docex = new DocExtraction(client);
            List<List<string>> batches = this.fp.BatchFiles(batchSize).ToList();
            int totalCount = batches.Count;
            int batchCount = 1;
            foreach (List<string> batch in batches)
            {
                Console.WriteLine($"Starting batch {batchCount} of {totalCount}");
                List<OnDoc> results = await docex.RunOCR(batch);
                foreach (OnDoc doc in results)
                {
                    this.fileTexts.Add(doc.GetFullText());
                }
                batchCount++;
            }
        }

        /// <summary>write classification CSV to disc</summary>
        /// <param name="filePath">Full path to write csv file to.</param>
        public void toCSV(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<DocRecord>();
                csv.NextRecord();
                for (int ind = 0; ind < this.filePaths.Count; ind++)
                {
                    DocRecord rec = new DocRecord
                    {
                        filename = Path.GetFileName(filePaths[ind]),
                        docClass = fileClasses[ind],
                        docText = fileTexts[ind]
                    };
                    csv.WriteRecord(rec);
                    csv.NextRecord();
                }
            }
        }

        /// <summary>gathers filepaths and sets classes. Must be run prior to createClassifier</summary>
        /// <param name="acceptedTypes">Acceptable file extensions. Defaults to this.acceptedTypes</param>
        public void setFileStructure(List<string> acceptedTypes = null)
        {
            this.acceptedTypes = acceptedTypes ?? this.acceptedTypes;
            fp.GetFilePathsFromDir(this.directoryPath, this.acceptedTypes, false);
            this.filePaths = fp.filePaths;
            this.checkIfFilesEmpty();
            this.fileClasses = fp.GetParentDirectoriesOfFilepaths();
            this.validateMoreThanOneClass();
        }

        private void checkIfFilesEmpty()
        {
            if (this.filePaths.Count == 0)
            {
                throw new ArgumentException(@$"No valid files found in {this.directoryPath} with suffixes
                                               ending in {this.acceptedTypes}");
            }
        }

        private void validateMoreThanOneClass()
        {
            if (this.GetClasses().Count < 2)
            {
                throw new ArgumentException(@$"You must have at least two subdirectories in 
                                              {this.directoryPath}, only found files in {this.GetClasses()}");
            }
        }

        public List<string> GetClasses()
        {
            return this.fileClasses.Distinct().ToList();
        }
    }
}