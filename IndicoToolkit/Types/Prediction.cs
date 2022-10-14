using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using IndicoToolkit.Exception;

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
        public int? Start { get; set; }
        [JsonProperty("end")]
        public int? End { get; set; }
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
        /// AutoReview params
        public bool Rejected { get; set; }
        public bool Accepted { get; set; }
        public string Error { get; set; }
        /// Position params
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public float BbTop { get; set; }
        public float BbBot { get; set; }
        public float BbLeft { get; set; }
        public float BbRight { get; set; }
        public int RowNumber { get; set; }
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
            string error = "",
            int top = -1,
            int bottom = -1,
            int left = -1,
            int right = -1,
            float bbTop = -1f,
            float bbBot = -1f,
            float bbLeft = -1f,
            float bbRight = -1f,
            int rowNumber = 0
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
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
            BbTop = bbTop;
            BbBot = bbBot;
            BbLeft = bbLeft;
            BbRight = bbRight;
            RowNumber = rowNumber;
        }

        public Tuple<float, float> getCorner(string leftOrRight, string topOrBot)
        {
            if ((leftOrRight != "bbRight" && leftOrRight != "bbLeft") || (topOrBot != "bbTop" && topOrBot != "bbBot"))
            {
                throw new ToolkitInputException(
                    "Invalid input. Ensure the first arg is 'bbRight' or 'bbLeft' and the second arg is 'bbTop' or 'bbBot'"
                );
            }
            float leftOrRightVal = leftOrRight == "bbRight" ? BbRight : BbLeft;
            float topOrBotVal = topOrBot == "bbTop" ? BbTop : BbBot;
            return new Tuple<float, float>(leftOrRightVal, topOrBotVal);
        }
    }


}