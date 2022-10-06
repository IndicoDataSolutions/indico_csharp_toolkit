using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using IndicoToolkit.Exception;

namespace IndicoToolkit.Types
{
    /// <summary>
    /// Class <c>MappedPrediction</c> represents a prediction with positional data appended to it.
    /// </summary>
    public class MappedPrediction : Prediction
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public float BbTop { get; set; }
        public float BbBot { get; set; }
        public float BbLeft { get; set; }
        public float BbRight { get; set; }

        public MappedPrediction(
            int start,
            int end,
            int pageNum,
            string label,
            string text,
            Dictionary<string, float> confidence,
            Grouping[] groupings,
            int top,
            int bottom,
            int left,
            int right,
            float bbTop,
            float bbBot,
            float bbLeft,
            float bbRight
        ) : base(start, end, pageNum, label, text, confidence, groupings)
        {
            this.Top = top;
            this.Bottom = bottom;
            this.Left = left;
            this.Right = right;
            this.BbTop = bbTop;
            this.BbBot = bbBot;
            this.BbLeft = bbLeft;
            this.BbRight = bbRight;
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