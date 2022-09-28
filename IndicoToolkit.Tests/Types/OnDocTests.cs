using Xunit;
using IndicoToolkit.Types;
using System.Collections.Generic;

namespace IndicoToolkit.Tests
{
    public class OnDocTests 
    {
        static dynamic ondocJson = Utils.LoadJson("data/simple_ondoc.json");
        
        [Fact]
        public void TestGetPageTexts()
        {
            List<OnDocPage> rawResult = ondocJson.ToObject<List<OnDocPage>>();
            OnDoc ondoc = new OnDoc(rawResult);
            List<string> pages = ondoc.GetPageTexts();
            Assert.Equal(2, pages.Count);
            Assert.True(pages[0].Contains("Page one"));
            Assert.True(pages[1].Contains("Page 2"));
        }

        [Fact]
        public void TestGetFullText()
        {
            List<OnDocPage> rawResult = ondocJson.ToObject<List<OnDocPage>>();
            OnDoc ondoc = new OnDoc(rawResult);
            string fullText = ondoc.GetFullText();
            Assert.True(fullText.Contains("Page one"));
            Assert.True(fullText.Contains("Page 2"));        }
    }
}