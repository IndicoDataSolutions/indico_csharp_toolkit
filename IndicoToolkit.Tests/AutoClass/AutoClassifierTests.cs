using Xunit;
using IndicoToolkit.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using CsvHelper;


namespace IndicoToolkit.Tests
{
    public class AutoClassifierTests 
    {
        static string targetPath = Path.Join(Utils.file_dir, "data/auto_class/");
        static string badFilesPath = Path.Join(Utils.file_dir, "data/auto_class/empty/");
        static string badClassPath = Path.Join(Utils.file_dir, "data/auto_class/a/");
        
        [Fact]
        public void TestSetFileStructure()
        {
            AutoClassifier auto = new AutoClassifier(Utils.client, targetPath);
            auto.setFileStructure();
            Assert.Equal(1, auto.fileClasses.Where(s=> s == "b").Count());
            Assert.Equal(2, auto.fileClasses.Where(s=> s == "a").Count());
            Assert.Equal(3, auto.filePaths.Count());
        }
        
        [Fact]
        public void TestSetFileStructureNoFiles()
        {
            AutoClassifier auto = new AutoClassifier(Utils.client, badFilesPath);
            Assert.Throws<ArgumentException>(() => auto.setFileStructure());
        }

        [Fact]
        public void TestSetFileStructureOnlyOneClass()
        {
            AutoClassifier auto = new AutoClassifier(Utils.client, badClassPath);
            Assert.Throws<ArgumentException>(() => auto.setFileStructure());
        }

        [Fact]
        public async Task TestCreateClassifier()
        {
            AutoClassifier auto = new AutoClassifier(Utils.client, targetPath);
            auto.setFileStructure();
            await auto.setDocumentText(2);
            Assert.Equal(1, auto.fileClasses.Where(s=> s == "b").Count());
            Assert.Equal(2, auto.fileClasses.Where(s=> s == "a").Count());
            Assert.Equal(3, auto.fileTexts.Count());
            Assert.Equal(2, auto.fileTexts.Where(s=> s.Contains("YY")).Count());
        }

        [Fact]
        public async Task TestToCSV()
        {
            AutoClassifier auto = new AutoClassifier(Utils.client, targetPath);
            auto.setFileStructure();
            await auto.setDocumentText();
            string myTempFile = Path.GetTempFileName();
            auto.toCSV(myTempFile);
            int shouldBeTwoAclass = 0;
            int shouldBeOneBclass = 0;
            using (var reader = new StreamReader(myTempFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                List<DocRecord> records = csv.GetRecords<DocRecord>().ToList();
                Assert.Equal(3, records.Count());
                foreach(DocRecord rec in records)
                {
                    Assert.Contains(".pdf", rec.filename);
                    if (rec.filename.Contains("yy"))
                    {
                        Assert.Equal("a", rec.docClass);
                        Assert.Contains("YY", rec.docText);
                        Assert.Contains("ZZ", rec.docText);
                        shouldBeTwoAclass ++;
                    } else {
                        Assert.Equal("b", rec.docClass);
                        Assert.DoesNotContain("YY", rec.docText);
                        Assert.DoesNotContain("ZZ", rec.docText);
                        shouldBeOneBclass ++;
                    }
                }
            }
            Assert.Equal(1, shouldBeOneBclass);
            Assert.Equal(2, shouldBeTwoAclass);
        }
    }
}