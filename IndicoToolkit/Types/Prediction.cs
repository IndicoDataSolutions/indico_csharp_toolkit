using System.Collections.Generic;
using Newtonsoft.Json;

namespace IndicoToolkit.Types
{
    public class Grouping
    {
        [JsonProperty("group_name")]
        public string GroupName { get; set; }
        [JsonProperty("group_index")]
        public int GroupIndex { get; set; }
        [JsonProperty("group_id")]
        public string GroupId { get; set; }

        public Grouping(string groupName, int groupIndex, string groupId)
        {
            GroupName = groupName;
            GroupIndex = groupIndex;
            GroupId = groupId;
        }
    }

    public class Prediction
    {
        [JsonProperty("start")]
        public int Start { get; set; }
        [JsonProperty("end")]
        public int End { get; set; }
        [JsonProperty("page_num")]
        public int PageNum { get; set; }
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("confidence")]
        public Dictionary<string, float> Confidence { get; set; }
        [JsonProperty("groupings")]
        public Grouping[] Groupings { get; set; }
        public bool Rejected { get; set; }
        public bool Accepted { get; set; }
        public string Error { get; set; }
        public Prediction(
            int start,
            int end,
            int pageNum,
            string label,
            string text,
            Dictionary<string, float> confidence,
            Grouping[] groupings,
            bool rejected = false,
            bool accepted = false,
            string error = ""
        )
        {
            Start = start;
            End = end;
            PageNum = pageNum;
            Label = label;
            Text = text;
            Confidence = confidence;
            Groupings = groupings;
            Rejected = rejected;
            Accepted = accepted;
            Error = error;
        }
    }


}