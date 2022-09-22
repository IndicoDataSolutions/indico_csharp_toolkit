using System;
using System.Linq;
using System.Collections.Generic;

using IndicoToolkit.Types;
using IndicoToolkit.Exception;

namespace IndicoToolkit.Association
{
    /// <summary>
    /// Class <c>Positioning</c> helps identify relative positions in a document using bounding box data.
    /// Positions are expected to contain, at a minimum, the following top-level keys: "bbTop", "bbBot", "bbLeft", "bbRight", "page_num".
    /// </summary>
    public class Positioning
    {

        public Positioning() { }

        /// <summary>
        /// Check if the location of one box is above another.
        /// </summary>
        /// <param name="abovePos">The position expected to be above</param>
        /// <param name="belowPos">The position expected to be below</param>
        /// <param name="mustBeSamePage">Required to be on same page. Defaults to true.</param>
        public bool positionedAbove(Position abovePos, Position belowPos, bool mustBeSamePage = true)
        {
            bool isAbove = false;
            if (belowPos.pageNum < abovePos.pageNum)
            {
                isAbove = false;
            }
            else if (belowPos.pageNum != abovePos.pageNum && mustBeSamePage)
            {
                isAbove = false;
            }
            else if (xAxisOverlap(abovePos, belowPos))
            {
                if (belowPos.pageNum == abovePos.pageNum && !yAxisAbove(abovePos, belowPos))
                {
                    isAbove = false;
                }
                else
                {
                    isAbove = true;
                }
            }
            return isAbove;
        }

        /// <summary>
        /// Check if the location of one box is on the same page and above another and that the boxes' x-axis positions overlap by at least "minOverlapPercent".
        /// </summary>
        /// <param name="abovePos">The position excepted to be above</param>
        /// <param name="belowPos">The position excepted to be below</param>
        /// <param name="minOverlapPercent">The minimum amount of overlap needed. Defaults to null.</param>
        public bool positionedAboveOverlap(Position abovePos, Position belowPos, float? minOverlapPercent = null)
        {
            bool isAbove = false;
            bool isMinOverlap = true;
            if (belowPos.pageNum != abovePos.pageNum)
            {
                return false;
            }
            if (xAxisOverlap(abovePos, belowPos) && yAxisAbove(abovePos, belowPos))
            {
                isAbove = true;
                float overlapAmount = getHorizontalOverlap(abovePos, belowPos);
                if (minOverlapPercent != null && overlapAmount < minOverlapPercent)
                {
                    isMinOverlap = false;
                }
            }
            return isAbove && isMinOverlap;
        }

        /// <summary>
        /// Check if two box positions are located on the same level/row, i.e. yaxes overlap.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        /// <param name="mustBeSamePage">Required to be on same page. Defaults to true.</param>
        public bool positionedOnSameLevel(Position pos1, Position pos2, bool mustBeSamePage = true)
        {
            bool sameLevel = false;
            if (mustBeSamePage && !onSamePage(pos1, pos2))
            {
                sameLevel = false;
            }
            else if (yAxisOverlap(pos1, pos2))
            {
                sameLevel = true;
            }
            return sameLevel;
        }

        /// <summary>
        /// Get the minimum distance between any two corners of two bounding boxes via the pythagorean formula.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        /// <param name="pageHeight">If you want to measure distance across pages, set the OCR page height otherwise locations on separate pages will raise an exception. Defaults to -1.</param>
        public float getMinDistance(Position pos1, Position pos2, int? pageHeight = null)
        {
            bool addPageHeight = false;
            int pageDifference = Math.Abs(pos1.pageNum - pos2.pageNum);
            if (pageDifference > 0)
            {
                if (pageHeight == null)
                {
                    throw new ToolkitInputException(
                        "Predictions are not on the same page! Must enter a page height"
                    );
                }
                else
                {
                    addPageHeight = true;
                }
            }
            List<float> distances = new List<float>();
            List<Tuple<string, string>> corners = new List<Tuple<string, string>>() {
                new Tuple<string, string>("bbRight", "bbTop"),
                new Tuple<string, string>("bbRight", "bbBot"),
                new Tuple<string, string>("bbLeft", "bbTop"),
                new Tuple<string, string>("bbLeft", "bbBot"),
            };
            foreach (Tuple<string, string> p1 in corners)
            {
                foreach (Tuple<string, string> p2 in corners)
                {
                    float distance = distanceBetweenPoints(
                        pos1.getCorner(p1.Item1, p1.Item2),
                        pos2.getCorner(p2.Item1, p2.Item2)
                    );
                    distances.Add(distance);
                }
            }
            float minDistance = distances.Min();
            if (addPageHeight)
            {
                minDistance += (int)pageHeight * pageDifference;
            }
            return minDistance;
        }

        /// <summary>
        /// Get the amount of horizontal overlap between two bounding boxes.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        public float getHorizontalOverlap(Position pos1, Position pos2)
        {
            int pageDifference = Math.Abs(pos1.pageNum - pos2.pageNum);
            if (pageDifference > 0)
            {
                throw new ToolkitInputException(
                    "Predictions are not on the same page!"
                );
            }
            if (xAxisOverlap(pos1, pos2))
            {
                float horizontalOverlapDistance = Math.Abs(
                    Math.Max(pos1.bbLeft, pos2.bbLeft) - Math.Min(pos1.bbRight, pos2.bbRight)
                );
                float positionWidth = Math.Abs(pos2.bbLeft - pos2.bbRight);
                return horizontalOverlapDistance / positionWidth;
            }
            else
            {
                return 0f;
            }
        }

        /// <summary>
        /// Get the amount of vertical overlap between two bounding boxes (i.e. percentage of y-axis overlap).
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        public float getVerticalOverlap(Position pos1, Position pos2)
        {
            int pageDifference = Math.Abs(pos1.pageNum - pos2.pageNum);
            if (pageDifference > 0)
            {
                throw new ToolkitInputException(
                    "Predictions are not on the same page!"
                );
            }
            if (yAxisOverlap(pos1, pos2))
            {
                float verticalOverlapDistance = Math.Abs(
                    Math.Max(pos1.bbTop, pos2.bbTop) - Math.Min(pos1.bbBot, pos2.bbBot)
                );
                float positionHeight = Math.Abs(pos2.bbTop - pos2.bbBot);
                return verticalOverlapDistance / positionHeight;
            }
            else
            {
                return 0f;
            }
        }

        /// <summary>
        /// Get the vertical minimum distance between two bounding boxes. Returns negative distance if belowPos is positioned above abovePos.
        /// </summary>
        /// <param name="abovePos">The position expected to be above</param>
        /// <param name="belowPos">The position expected to be below</param>
        /// <param name="pageHeight">If you want to measure distances across pages, set the OCR page height otherwise locations on separate pages will raise an exception. Defaults to null.</param>
        public float getVerticalMinDistance(Position abovePos, Position belowPos, int? pageHeight = null)
        {
            bool addPageHeight = false;
            int pageDifference = Math.Abs(abovePos.pageNum - belowPos.pageNum);
            if (pageDifference > 0)
            {
                if (pageHeight == null)
                {
                    throw new ToolkitInputException(
                        "Predictions are not on the same page! Must enter a page height."
                    );
                }
                else
                {
                    addPageHeight = true;
                }
            }
            float minDistance = belowPos.bbTop - abovePos.bbBot;
            if (addPageHeight)
            {
                minDistance += (int)pageHeight * pageDifference;
            }
            return minDistance;
        }

        /// <summary>
        /// Get the horizontal minimum distance between two bounding boxes. Returns 0 if bounding boxes are adjacent or overlap.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        public float getHorizontalMinDistance(Position pos1, Position pos2)
        {
            int pageDifference = Math.Abs(pos1.pageNum - pos2.pageNum);
            if (pageDifference > 0)
            {
                throw new ToolkitInputException(
                    "Predictions are not on the same page! Must enter a page height."
                );
            }
            if (xAxisOverlap(pos1, pos2))
            {
                return 0f;
            }
            float minDistance1 = Math.Abs(pos1.bbLeft - pos2.bbRight);
            float minDistance2 = Math.Abs(pos1.bbRight - pos2.bbLeft);
            return Math.Min(minDistance1, minDistance2);
        }

        /// <summary>
        /// Get the distance between two points (x, y).
        /// </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        public float distanceBetweenPoints(Tuple<float, float> point1, Tuple<float, float> point2)
        {
            float x = (float)Math.Pow(point1.Item1 - point2.Item1, 2);
            float y = (float)Math.Pow(point1.Item2 - point2.Item2, 2);
            return (float)Math.Sqrt(x + y);
        }

        /// <summary>
        /// Get the manhattan distance between two points (x, y).
        /// </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        public float manhattanDistanceBetweenPoints(Tuple<float, float> point1, Tuple<float, float> point2)
        {
            float x = Math.Abs(point1.Item1 - point2.Item1);
            float y = Math.Abs(point1.Item2 - point2.Item2);
            return x + y;
        }

        /// <summary>
        /// Check whether two locations overlap on the yaxis, i.e. same "row".
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        public bool yAxisOverlap(Position pos1, Position pos2)
        {
            return pos2.bbBot > pos1.bbTop && pos2.bbTop < pos1.bbBot;
        }

        /// <summary>
        /// Check whether two locations overlap on the xaxis.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        public bool xAxisOverlap(Position pos1, Position pos2)
        {
            return pos1.bbLeft < pos2.bbRight && pos2.bbLeft < pos1.bbRight;
        }

        /// <summary>
        /// Check whether the first position is above the second.
        /// </summary>
        /// <param name="abovePos">The position excepted to be above</param>
        /// <param name="belowPos">The position excepted to be below</param>
        public bool yAxisAbove(Position abovePos, Position belowPos)
        {
            return abovePos.bbBot < belowPos.bbTop;
        }

        /// <summary>
        /// Check whether the two positions are on the same page.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        public bool onSamePage(Position pos1, Position pos2)
        {
            return pos1.pageNum == pos2.pageNum;
        }
    }
}