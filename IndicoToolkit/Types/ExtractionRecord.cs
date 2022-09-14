using System.Collections.Generic;

namespace IndicoToolkit.Types
{
    public class FullExtractionRecord
    {
        public string start { get; set; }
        public string end { get; set; }
        public string label { get; set; }
        public string text { get; set; }
        public string confidence { get; set; }
    }

    public class ExtractionRecord
    {
        public string label { get; set; }
        public string text { get; set; }
        public string confidence { get; set; }
    }
}