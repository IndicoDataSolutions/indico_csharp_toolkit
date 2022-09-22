using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace IndicoToolkit.Types
{
    public class ExtractionRecord
    {
        public string? Start { get; set; }
        public string? End { get; set; }
        public string Label { get; set; }
        public string Text { get; set; }
        public string Confidence { get; set; }

        public bool ShouldSerializeStart()
        {
            return (Start is not null);
        }

        public bool ShouldSerializeEnd()
        {
            return (End is not null);
        }
    }
}