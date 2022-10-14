using Newtonsoft.Json;

namespace IndicoToolkit.Types
{

    public class OcrToken
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("doc_offset")]
        public DocOffset DocOffset { get; set; }

        [JsonProperty("position")]
        public Position Position { get; set; }

        [JsonProperty("page_num")]
        public int PageNum { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("prediction_index")]
        public int PredictionIndex { get; set; }

        public OcrToken(
            string text,
            DocOffset docOffset,
            Position position,
            int pageNum,
            string label,
            int predictionIndex)
        {
            Text = text;
            DocOffset = docOffset;
            Position = position;
            PageNum = pageNum;
            Label = label;
            PredictionIndex = predictionIndex;
        }

    }


}