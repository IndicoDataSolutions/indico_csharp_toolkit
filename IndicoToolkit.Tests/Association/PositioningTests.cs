using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;

using IndicoToolkit.Association;
using IndicoToolkit.Types;

namespace IndicoToolkit.Tests
{
    public class PositioningFixture: IDisposable
    {
        public Position TestAbovePosition { get; private set; }
        public Position TestBelowPosition { get; private set; }
        public PositioningFixture()
        {
            TestAbovePosition = new Position(
                top : 1,
                bottom : 10,
                left : 5,
                right : 15,
                bbTop : 1f,
                bbBot : 10f,
                bbLeft : 5f,
                bbRight : 15f,
                pageNum : 0
            );
            TestBelowPosition = new Position(
                top : 15,
                bottom : 25,
                left : 5,
                right : 15,
                bbTop : 15f,
                bbBot : 25f,
                bbLeft : 5f,
                bbRight : 15f,
                pageNum : 0
            );
        }

        public void Dispose()
        {
            // ... clean up test data if needed
        }

    }
    
    public class PositioningTests
    {
        PositioningFixture Fixture { get; }

        public PositioningTests(PositioningFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void PositionedAbove_MustBeSamePageTrue_True()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Fixture.TestAbovePosition;
            Position belowPos = Fixture.TestBelowPosition;
            bool isAbove = positioning.positionedAbove(abovePos, belowPos);
            Assert.True(isAbove); 
        }

        [Fact]
        public void PositionedAbove_MustBeSamePageTrue_False()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Fixture.TestAbovePosition;
            Position belowPos = Fixture.TestBelowPosition;
            bool isAbove = positioning.positionedAbove(belowPos, abovePos);
            Assert.False(isAbove); 
        }

        [Fact]
        public void PositionedAbove_MustBeSamePageFalse_True()
        {
            Positioning positioning = new Positioning();
            Position abovePos = Fixture.TestAbovePosition;
            Position belowPos = Fixture.TestBelowPosition;
            abovePos.pageNum = 10;
            bool isAbove = positioning.positionedAbove(abovePos, belowPos, mustBeSamePage : false);
            Assert.True(isAbove); 
        }

    }
}