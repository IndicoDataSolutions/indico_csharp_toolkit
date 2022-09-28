using Xunit;
using System;

using IndicoToolkit.Types;

namespace IndicoToolkit.Tests
{   
    public class PositionTests
    {

        public PositionTests() { }

        [Fact]
        public void Init_EnsureGetProperties()
        {
            Position pos = Utils.createPosition();
            Assert.Equal(pos.top, 1);
            Assert.Equal(pos.bottom, 10);
            Assert.Equal(pos.left, 5);
            Assert.Equal(pos.right, 15);
            Assert.Equal(pos.bbTop, 1f);
            Assert.Equal(pos.bbBot, 10f);
            Assert.Equal(pos.bbLeft, 5f);
            Assert.Equal(pos.bbRight, 15f);
            Assert.Equal(pos.pageNum, 0);
        }

        [Fact]
        public void GetCorner_TopRight()
        {
            Position pos = Utils.createPosition();
            Tuple<float, float> topRight = pos.getCorner("bbRight", "bbTop");
            Assert.Equal(topRight.Item1, 15f);
            Assert.Equal(topRight.Item2, 1f);
        }

        [Fact]
        public void GetCorner_BotRight()
        {
            Position pos = Utils.createPosition();
            Tuple<float, float> topRight = pos.getCorner("bbRight", "bbBot");
            Assert.Equal(topRight.Item1, 15f);
            Assert.Equal(topRight.Item2, 10f);
        }

        [Fact]
        public void GetCorner_TopLeft()
        {
            Position pos = Utils.createPosition();
            Tuple<float, float> topRight = pos.getCorner("bbLeft", "bbTop");
            Assert.Equal(topRight.Item1, 5f);
            Assert.Equal(topRight.Item2, 1f);
        }

        [Fact]
        public void GetCorner_BotLeft()
        {
            Position pos = Utils.createPosition();
            Tuple<float, float> topRight = pos.getCorner("bbLeft", "bbBot");
            Assert.Equal(topRight.Item1, 5f);
            Assert.Equal(topRight.Item2, 10f);
        }
    }
}