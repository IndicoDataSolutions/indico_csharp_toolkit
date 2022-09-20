using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace IndicoToolkit.Association
{
    public class Position
    {
        public float bbTop {get; set;}
        public float bbBot {get; set;}
        public float bbLeft {get; set;}
        public float bbRight {get; set;}
        public int pageNum {get; set;}

        public Position(float bbTop, float bbBot, float bbLeft, float bbRight, int pageNum)
        {
            this.bbTop = bbTop;
            this.bbBot = bbBot;
            this.bbLeft = bbLeft;
            this.bbRight = bbRight;
            this.pageNum = pageNum;
        }
    }

    
}