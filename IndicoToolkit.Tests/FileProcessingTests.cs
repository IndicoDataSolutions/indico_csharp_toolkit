using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace IndicoToolkit.Tests
{
    public class FileProcessingTest
    {
        [Fact]
        public void TestGetFilePathsFromEmptyDir()
        {
            FileProcessing fptest = new FileProcessing();
            fptest.GetFilePathsFromDir(Path.Join(Utils.file_dir, "data/empty/"));
            Assert.Equal(0, fptest.filePaths.Count);
        }

        [Fact]
        public void TestGetFilePathsFromDirDefault()
        {
            string pathToDir = Path.Join(Utils.file_dir, "data/samples/"); 
            FileProcessing fptest = new FileProcessing();
            fptest.GetFilePathsFromDir(pathToDir, null, true);
            Assert.Equal(1, fptest.filePaths.Count);
        }

        [Fact]
        public void TestGetFilePathsFromDirWithAcceptedType()
        {
            string pathToDir = Path.Join(Utils.file_dir, "data/samples/"); 
            List<string> acceptedType = new List<string>() {"*.pdf","*.json"};
            FileProcessing fptest = new FileProcessing();
            fptest.GetFilePathsFromDir(pathToDir, acceptedType);
            Assert.Equal(3, fptest.filePaths.Count());
        }

        [Fact]
        public void TestGetFilePathsFromDirWithSubDir()
        {
            string pathToDir = Path.Join(Utils.file_dir, "data/"); 
            List<string> acceptedType = new List<string>() {"*.pdf","*.json"};
            FileProcessing fptest = new FileProcessing();
            fptest.GetFilePathsFromDir( pathToDir, acceptedType, false);
            Assert.Equal(8, fptest.filePaths.Count());
        }

        [Fact]
        public void TestBatchFiles()
        {
            List<string> filePaths = new List<string>() { "1", "2", "3", "4", "5"};
            FileProcessing fptest = new FileProcessing(filePaths);
            List<List<string>> batches = fptest.BatchFiles(2).ToList();
            // assert that we have the right number of batches
            Assert.Equal(2, batches[0].Count());
            Assert.Contains("1", batches[0]);
            Assert.Contains("2", batches[0]);
            Assert.Equal(2, batches[1].Count());
            Assert.Contains("3", batches[1]);
            Assert.Contains("4", batches[1]);
            Assert.Equal(1, batches[2].Count());
            Assert.Contains("5", batches[2]);
        }

        [Fact]
        public void TestGetParentDirectoriesOfFilepaths()
        {
            string pathToDir = Path.Join(Utils.file_dir, "data/samples/"); 
            List<string> acceptedType = new List<string>() {"*.pdf", "*.json", "*.csv"};
            FileProcessing fptest = new FileProcessing();
            fptest.GetFilePathsFromDir(pathToDir, acceptedType);
            List<string> directories = fptest.GetParentDirectoriesOfFilepaths();
            Assert.Equal(4, directories.Count());
            foreach(string parent in directories){
                Assert.Equal("samples", parent);
            }
        }
    }
}
