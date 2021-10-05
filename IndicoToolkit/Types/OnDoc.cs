using System.Linq;
using System.Collections.Generic;

namespace IndicoToolkit.Types
{
    public class OnDoc {

        public List<OnDocPage> ondoc {get; set;}

        public OnDoc(List<OnDocPage> ondocResult){
            ondoc = ondocResult;
        }
        public List<string> GetPageTexts(){
            return ondoc.Select(s => s.pages.First().text).ToList();
        }

        public string GetFullText(){
            List<string> page_texts = GetPageTexts();
            return string.Join("\n", page_texts.ToArray());
        }

    }
    public class OnDocPage
    {
        public List<Page> pages { get; set; }
        public List<Block> blocks { get; set; }
        public List<Token> tokens { get; set; }
        public List<Char> chars { get; set; }

    }

    public class Image
    {
        public string url { get; set; }
    }

    public class Dpi
    {
        public int dpix { get; set; }
        public int dpiy { get; set; }
    }

    public class Thumbnail
    {
        public string url { get; set; }
    }

    public class Size
    {
        public int height { get; set; }
        public int width { get; set; }
    }

    public class DocOffset
    {
        public int start { get; set; }
        public int end { get; set; }
    }

    public class OcrStatistics
    {
        public double mean_confidence { get; set; }
        public double median_confidence { get; set; }
        public int mode_confidence { get; set; }
    }

    public class Page
    {
        public Image image { get; set; }
        public Dpi dpi { get; set; }
        public Thumbnail thumbnail { get; set; }
        public Size size { get; set; }
        public DocOffset doc_offset { get; set; }
        public string text { get; set; }
        public int page_num { get; set; }
        public OcrStatistics ocr_statistics { get; set; }
    }

    public class Position
    {
        public int top { get; set; }
        public int bottom { get; set; }
        public int left { get; set; }
        public int right { get; set; }
        public int bbBot { get; set; }
        public int bbTop { get; set; }
        public int bbLeft { get; set; }
        public int bbRight { get; set; }
    }

    public class PageOffset
    {
        public int start { get; set; }
        public int end { get; set; }
    }

    public class Block
    {
        public string block_type { get; set; }
        public Position position { get; set; }
        public PageOffset page_offset { get; set; }
        public DocOffset doc_offset { get; set; }
        public string text { get; set; }
        public int page_num { get; set; }
    }

    public class BlockOffset
    {
        public int start { get; set; }
        public int end { get; set; }
    }

    public class Style
    {
        public bool underlined { get; set; }
        public bool bold { get; set; }
        public bool italic { get; set; }
        public string font_face { get; set; }
        public int font_size { get; set; }
        public string background_color { get; set; }
        public string text_color { get; set; }
    }

    public class Token
    {
        public Position position { get; set; }
        public BlockOffset block_offset { get; set; }
        public PageOffset page_offset { get; set; }
        public DocOffset doc_offset { get; set; }
        public string text { get; set; }
        public Style style { get; set; }
        public int page_num { get; set; }
    }

    public class Char
    {
        public int block_index { get; set; }
        public int page_index { get; set; }
        public Position position { get; set; }
        public int doc_index { get; set; }
        public int confidence { get; set; }
        public string text { get; set; }
        public int page_num { get; set; }
    }


}