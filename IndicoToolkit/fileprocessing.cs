using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace IndicoToolkit
{
    public class FileProcessing
    {
        public List<string> FilePaths; 
        public List<string> DefaultAcceptedTypes; 
        
        public FileProcessing ( List<string> filePathsInput = null)
        {
            FilePaths = filePathsInput ?? new List<string>();
            DefaultAcceptedTypes = new List<string>() {"*.pdf", "*.doc", "*.docx", "*.tiff", "*.tif", "*.png"};
        }

        public void GetFilePathsFromDir(string pathToDir,
                                        IEnumerable<string> acceptedTypes = null,
                                        bool topDirectoryOnly = true)
        {
            acceptedTypes = acceptedTypes ?? DefaultAcceptedTypes; 
            SearchOption searchOption = SearchOption.TopDirectoryOnly;
            if (!topDirectoryOnly)
            {
                searchOption = SearchOption.AllDirectories;
            }

            FilePaths = FileSearch( pathToDir, acceptedTypes.Distinct().ToList(), searchOption);
            
            // Print statment about files added or not added
            int dirSize = Directory.GetFiles(pathToDir, "*", searchOption).Length;
            Console.WriteLine($"Found {FilePaths.Count} valid files and {dirSize - FilePaths.Count} paths with invalid suffixes. Valid suffix:");
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
            for (int i = 0; i < FilePaths.Count; i = i + batchSize)
            {
                int end = Math.Min(batchSize, FilePaths.Count - i);
                yield return FilePaths.GetRange(i, end);
            }
        }

        public List<string> GetParentDirectoriesOfFilepaths()
        {
            List<string> directories = new List<string>();
            foreach(var file in FilePaths)
            {
                directories.Add(Directory.GetParent(file).Name);
            }
            return directories;
        }
    }
}
