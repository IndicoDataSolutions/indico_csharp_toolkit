using Xunit;
using IndicoToolkit.Types;
using System.Collections.Generic;
using System.IO;

namespace IndicoToolkit.Tests
{
    public class DocExtractionTests 
    {
        static string pdfPath = Path.Join(Utils.file_dir, "data/simple_doc.pdf");
        
        [Fact]
        public async void TestRunOCR(){
            DocExtraction docx = new DocExtraction(Utils.client);
            List<string> fpaths = new List<string>{pdfPath};
            List<OnDoc> results = await docx.RunOCR(fpaths);
            Assert.Equal(1, results.Count);
            Assert.Equal(2, results[0].GetPageTexts().Count);
            Assert.True(results[0].GetFullText().Contains("Page one"));
            Assert.True(results[0].GetFullText().Contains("Page 2"));
        }
    }
}