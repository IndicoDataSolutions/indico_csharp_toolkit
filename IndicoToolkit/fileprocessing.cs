using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace IndicoToolkit
{
    public class FileProcessing
    {
        public List<string> filePaths;
        public List<string> defaultAcceptedTypes = new List<string>() { "*.pdf", "*.doc", "*.docx", "*.tiff", "*.tif", "*.png" };

        public FileProcessing(List<string> filePathsInput = null)
        {
            this.filePaths = filePathsInput ?? new List<string>();
        }

        public void GetFilePathsFromDir(string pathToDir,
                                        IEnumerable<string> acceptedTypes = null,
                                        bool topDirectoryOnly = true)
        {
            acceptedTypes = acceptedTypes ?? defaultAcceptedTypes;
            SearchOption searchOption = SearchOption.TopDirectoryOnly;
            if (!topDirectoryOnly)
            {
                searchOption = SearchOption.AllDirectories;
            }

            this.filePaths = FileSearch(pathToDir, acceptedTypes.Distinct().ToList(), searchOption);
            int dirSize = Directory.GetFiles(pathToDir, "*", searchOption).Length;
            int filesFound = this.filePaths.Count;
            Console.WriteLine($"Found {filesFound} valid files and {dirSize - filesFound} paths with invalid suffixes.");
        }

        private List<string> FileSearch(string pathToDir,
                                        IEnumerable<string> acceptedTypes,
                                        SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            List<string> result = new List<string>();
            foreach (var acceptedType in acceptedTypes)
            {
                result.AddRange(Directory.GetFiles(pathToDir, acceptedType, searchOption));
            }

            return result;
        }

        public IEnumerable<List<string>> BatchFiles(int batchSize = 10)
        {
            for (int i = 0; i < filePaths.Count; i = i + batchSize)
            {
                int end = Math.Min(batchSize, filePaths.Count - i);
                yield return this.filePaths.GetRange(i, end);
            }
        }

        public List<string> GetParentDirectoriesOfFilepaths()
        {
            List<string> directories = new List<string>();
            foreach (var file in this.filePaths)
            {
                directories.Add(Directory.GetParent(file).Name);
            }
            return directories;
        }
    }
}
