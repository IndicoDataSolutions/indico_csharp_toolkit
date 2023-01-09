using System.Linq;
using System.Collections.Generic;

namespace IndicoToolkit.Types
{
    public class OnDocOCR
    {
        public List<OnDocOCRPage> pages { get; set; }
        public int num_pages { get; set; }

    }

    public class OnDocOCRPage
    {
        public int page_num { get; set; }
        public string image { get; set; }
        public string thumbnail { get; set; }
        public int doc_start_offset { get; set; }
        public int doc_end_offset { get; set; }
        public string page_info { get; set; }
    }
}