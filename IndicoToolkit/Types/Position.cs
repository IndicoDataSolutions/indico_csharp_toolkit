using System;

using IndicoToolkit.Exception;

namespace IndicoToolkit.Types
{
    public class Position
    {
        public int top { get; set; }
        public int bottom { get; set; }
        public int left { get; set; }
        public int right { get; set; }
        public float bbTop {get; set;}
        public float bbBot {get; set;}
        public float bbLeft {get; set;}
        public float bbRight {get; set;}
        public int pageNum {get; set;}

        public Position(
            int top,
            int bottom,
            int left,
            int right,
            float bbTop,
            float bbBot,
            float bbLeft,
            float bbRight,
            int pageNum
        )
        {
            this.top = top;
            this.bottom = bottom;
            this.left = left;
            this.right = right;
            this.bbTop = bbTop;
            this.bbBot = bbBot;
            this.bbLeft = bbLeft;
            this.bbRight = bbRight;
            this.pageNum = pageNum;
        }

        public Tuple<float, float> getCorner(string leftOrRight, string topOrBot)
        {
            if ((leftOrRight != "bbRight" && leftOrRight != "bbLeft") || (topOrBot != "bbTop" && topOrBot != "bbBot"))
            {
                throw new ToolkitInputException(
                    "Invalid input. Ensure the first arg is 'bbRight' or 'bbLeft' and the second arg is 'bbTop' or 'bbBot'"
                );
            }
            float leftOrRightVal = leftOrRight == "bbRight" ? bbRight : bbLeft; 
            float topOrBotVal = topOrBot == "bbTop" ? bbTop : bbBot;
            return new Tuple<float, float>(leftOrRightVal, topOrBotVal);
        }
    }

    
}