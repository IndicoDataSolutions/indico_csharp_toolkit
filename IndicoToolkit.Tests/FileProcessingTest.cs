using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace IndicoToolkit.Tests
{
    public class FileProcessingTest
    {
        [Fact]
        public void TestGetFilePathsFromDirNull()
        {
            FileProcessing fptest = new FileProcessing();
            Assert.True(fptest.FilePaths.Count == 0,
                $"Expected value of 0 but actually recieved {fptest.FilePaths.Count}");
        }

        [Fact]
        public void TestGetFilePathsFromDirDefault()
        {
            string pathToDir = Path.Join(Utils.file_dir, "data/samples/"); 
            FileProcessing fptest = new FileProcessing();
            fptest.GetFilePathsFromDir( pathToDir);
            Assert.True(fptest.FilePaths.Count == 1,
                $"Expected value of 1 but actually recieved {fptest.FilePaths.Count}");
        }

        [Fact]
        public void TestGetFilePathsFromDirWithAcceptedType()
        {
            string pathToDir = Path.Join(Utils.file_dir, "data/samples/"); 
            List<string> acceptedType = new List<string>() {"*.pdf","*.json"};
            FileProcessing fptest = new FileProcessing();
            fptest.GetFilePathsFromDir( pathToDir, acceptedType);
            Assert.True(fptest.FilePaths.Count == 3,
                $"Expected value of 3 but actually recieved {fptest.FilePaths.Count}");
        }

        [Fact]
        public void TestGetFilePathsFromDirWithSubDir()
        {
            string pathToDir = Path.Join(Utils.file_dir, "data/"); 
            List<string> acceptedType = new List<string>() {"*.pdf","*.json"};
            FileProcessing fptest = new FileProcessing();
            fptest.GetFilePathsFromDir( pathToDir, acceptedType, false);
            Assert.True(fptest.FilePaths.Count == 4,
                $"Expected value of 4 but actually recieved {fptest.FilePaths.Count}");
        }

        [Fact]
        public void TestBatchFiles()
        {
            List<string> filePaths = new List<string>() { "1", "2", "3", "4", "5"};
            FileProcessing fptest = new FileProcessing(filePaths);
            List<List<string>> batches = fptest.BatchFiles(4).ToList();
            // assert that we have the right number of batches
            Assert.True(batches[0].Count ==4,
                $"Expected value of 4 but actually recieved {batches[0].Count}");
            Assert.True(batches[1].Count ==1,
                $"Expected value of 1 but actually recieved {batches[1].Count}");
        }

        [Fact]
        public void TestGetParentDirectoriesOfFilepaths()
        {
            string pathToDir = Path.Join(Utils.file_dir, "data/samples/"); 
            List<string> acceptedType = new List<string>() {"*.pdf"};
            FileProcessing fptest = new FileProcessing();
            fptest.GetFilePathsFromDir( pathToDir, acceptedType );
            List<string> directories = fptest.GetParentDirectoriesOfFilepaths();
            Assert.True(directories[0] == "samples",
                $"Expected value of 'samples' but actually recieved {directories[0]}");
        }
    }
}
