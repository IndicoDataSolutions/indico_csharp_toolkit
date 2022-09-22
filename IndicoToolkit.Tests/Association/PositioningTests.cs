using Xunit;
using System;

using IndicoToolkit.Association;
using IndicoToolkit.Exception;
using IndicoToolkit.Types;

namespace IndicoToolkit.Tests
{
    public class PositioningTests
    {

        public PositioningTests() { }

        [Fact]
        public void PositionedAbove_MustBeSamePageTrue_ShouldBeTrue()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Utils.createPosition();
            Position belowPos = Utils.createPosition(bbTop: 11f, bbBot: 20f);
            bool isAbove = positioning.positionedAbove(abovePos, belowPos);
            Assert.True(isAbove);
        }

        [Fact]
        public void PositionedAbove_MustBeSamePageTrue_ShouldBeFalse()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Utils.createPosition();
            Position belowPos = Utils.createPosition(bbTop: 11f, bbBot: 20f);
            bool isAbove = positioning.positionedAbove(belowPos, abovePos);
            Assert.False(isAbove);
        }

        [Fact]
        public void PositionedAbove_MustBeSamePageFalse_ShouldBeTrue()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Utils.createPosition();
            Position belowPos = Utils.createPosition(bbTop: 11f, bbBot: 20f, pageNum: 10);
            bool isAbove = positioning.positionedAbove(abovePos, belowPos, mustBeSamePage: false);
            Assert.True(isAbove);
        }

        [Fact]
        public void PositionedAboveOverlap_IsNotSamePage_ShouldThrowException()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Utils.createPosition(pageNum: 2);
            Position belowPos = Utils.createPosition(bbTop: 11f, bbBot: 20f);
            Assert.Throws<ToolkitInputException>(() => positioning.positionedAboveOverlap(abovePos, belowPos));
        }

        [Fact]
        public void PositionedAboveOverlap_NotAbove_ShouldBeFalse()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Utils.createPosition();
            Position belowPos = Utils.createPosition(bbTop: 11f, bbBot: 20f);
            Assert.False(positioning.positionedAboveOverlap(belowPos, abovePos));
        }

        [Fact]
        public void PositionedAboveOverlap_NotXAxisOverlap_ShouldBeFalse()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Utils.createPosition();
            Position belowPos = Utils.createPosition(bbTop: 11f, bbBot: 20f, bbLeft: 16f, bbRight: 26f);
            Assert.False(positioning.positionedAboveOverlap(abovePos, belowPos));
        }

        [Fact]
        public void PositionedAboveOverlap_MinOverlapPercentNull_ShouldBeTrue()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Utils.createPosition();
            Position belowPos = Utils.createPosition(bbTop: 11f, bbBot: 20f);
            Assert.True(positioning.positionedAboveOverlap(abovePos, belowPos));
        }

        [Fact]
        public void PositionedAboveOverlap_NotAtLeastMinOverlapPercent_ShouldBeFalse()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Utils.createPosition();
            Position belowPos = Utils.createPosition(bbTop: 11f, bbBot: 20f, bbLeft: -2f, bbRight: 8f);
            Assert.False(positioning.positionedAboveOverlap(abovePos, belowPos, minOverlapPercent: .5f));
        }

        [Fact]
        public void PositionedAboveOverlap_MinOverlapPercent_ShouldBeTrue()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Utils.createPosition();
            Position belowPos = Utils.createPosition(bbTop: 11f, bbBot: 20f);
            Assert.True(positioning.positionedAboveOverlap(abovePos, belowPos, minOverlapPercent: .5f));
        }

        [Fact]
        public void PositionedOnSameLevel_MustBeSamePage_ShouldBeFalse()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition(pageNum: 2);
            Assert.False(positioning.positionedOnSameLevel(pos1, pos2));
        }

        [Fact]
        public void PositionedOnSameLevel_MustBeSamePage_ShouldBeTrue()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition();
            Assert.True(positioning.positionedOnSameLevel(pos1, pos2));
        }

        [Fact]
        public void PositionedOnSameLevel_MustBeSamePageFalse_ShouldBeTrue()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition(pageNum: 10);
            Assert.True(positioning.positionedOnSameLevel(pos1, pos2, mustBeSamePage: false));
        }

        [Fact]
        public void GetMinDistance_Simple_ShouldBeTrue()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition();
            Assert.Equal(positioning.getMinDistance(pos1, pos2), 0f);
        }

        [Fact]
        public void GetMinDistance_PageDifferenceNotZero_ShouldThrowException()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition(pageNum: 2);
            Assert.Throws<ToolkitInputException>(() => positioning.getMinDistance(pos1, pos2));
        }

        [Fact]
        public void GetVerticalMinDistance_Simple_ShouldBeEqual()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition(bbTop: 11, bbBot: 20);
            float verticalMinDistance = positioning.getVerticalMinDistance(pos1, pos2);
            Assert.Equal(verticalMinDistance, 1f);
        }

        [Fact]
        public void GetVerticalMinDistance_PageDifferenceNotZero_ShouldThrowException()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition(pageNum: 2);
            Assert.Throws<ToolkitInputException>(() => positioning.getVerticalMinDistance(pos1, pos2));
        }

        [Fact]
        public void GetVerticalMinDistance_PageDifferenceNotZero_PageHeightNotNull_ShouldBeEqual()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition(pageNum: 2);
            float verticalMinDistance = positioning.getVerticalMinDistance(pos1, pos2, pageHeight: 10);
            Assert.Equal(verticalMinDistance, 20f);
        }

        [Fact]
        public void GetHorizontalMinDistance_PageDifferenceNotZero_ShouldThrowException()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition(pageNum: 2);
            Assert.Throws<ToolkitInputException>(() => positioning.getHorizontalMinDistance(pos1, pos2));
        }

        [Fact]
        public void GetHorizontalMinDistance_ShouldBeEqual()
        {

        }

        [Fact]
        public void DistanceBetweenPoints_ShouldBeEqual()
        {
            Positioning positioning = new Positioning();
            Tuple<float, float> point1 = new Tuple<float, float>(0, 0);
            Tuple<float, float> point2 = new Tuple<float, float>(2, -2);
            float distance = positioning.distanceBetweenPoints(point1, point2);
            Assert.Equal(distance, (float)Math.Sqrt(8f));
        }

        [Fact]
        public void ManhattanDistanceBetweenPoints_ShouldBeEqual()
        {
            Positioning positioning = new Positioning();
            Tuple<float, float> point1 = new Tuple<float, float>(0, 5);
            Tuple<float, float> point2 = new Tuple<float, float>(5, -5);
            float manhattanDistance = positioning.manhattanDistanceBetweenPoints(point1, point2);
            Assert.Equal(manhattanDistance, 15f);
        }

        [Fact]
        public void YAxisOverlap_ShouldBeFalse()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition(bbTop: 11f, bbBot: 20f);
            Assert.False(positioning.yAxisOverlap(pos1, pos2));
        }

        [Fact]
        public void YAxisOverlap_ShouldBeTrue()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition();
            Assert.True(positioning.yAxisOverlap(pos1, pos2));
        }

        [Fact]
        public void XAxisOverlap_ShouldBeFalse()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition(bbLeft: 0f, bbRight: 5f);
            Assert.False(positioning.xAxisOverlap(pos1, pos2));
        }

        [Fact]
        public void XAxisOverlap_ShouldBeTrue()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition();
            Assert.True(positioning.xAxisOverlap(pos1, pos2));
        }

        [Fact]
        public void YAxisAbove_ShouldBeFalse()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Utils.createPosition();
            Position belowPos = Utils.createPosition();
            Assert.False(positioning.yAxisAbove(abovePos, belowPos));
        }

        [Fact]
        public void YAxisAbove_ShouldBeTrue()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Utils.createPosition();
            Position belowPos = Utils.createPosition(bbTop: 20f, bbBot: 30f);
            Assert.True(positioning.yAxisAbove(abovePos, belowPos));
        }

        [Fact]
        public void OnSamePage_ShouldBeFalse()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition(pageNum: 10);
            Assert.False(positioning.onSamePage(pos1, pos2));
        }

        [Fact]
        public void OnSamePage_ShouldBeTrue()
        {
            Positioning positioning = new Positioning();
            Position pos1 = Utils.createPosition();
            Position pos2 = Utils.createPosition();
            Assert.True(positioning.onSamePage(pos1, pos2));
        }
    }
}