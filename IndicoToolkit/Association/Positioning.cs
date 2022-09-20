using System.Linq;
using System.Collections.Generic;

namespace IndicoToolkit.Association
{
    /// <summary>
    /// Class <c>Positioning</c> helps identify relative positions in a document using bounding box data.
    /// Positions are expected to contain, at a minimum, the following top-level keys: "bbTop", "bbBot", "bbLeft", "bbRight", "page_num".
    /// </summary>
    public class Positioning {
        
        public Positioning()
        {}


        /// <summary>
        /// Check if the location of one box is above another.
        /// </summary>
        /// <param name="abovePos">The position expected to be above</param>
        /// <param name="belowPos">The position expected to be below</param>
        /// <param name="mustBeSamePage">Required to be on same page. Defaults to true.</param>
        public bool positionedAbove(Position abovePos, Position belowPos, bool mustBeSamePage = true)
        {
            return false;
        }

        /// <summary>
        /// Check if the location of one box is on the same page and above another if the lower box's overlap is at least the given percentage.
        /// </summary>
        /// <param name="abovePos">The position excepted to be above</param>
        /// <param name="belowPos">The position excepted to be below</param>
        /// <param name="minOverlapPercent">The minimum amount of overlap needed. Defaults to null.</param>
        public bool positionedAboveOverlap(Position abovePos, Position belowPos, float minOverlapPercent = null)
        {
            return false;
        }

        /// <summary>
        /// Check if two box positions are located on the same level/row, i.e. yaxes overlap.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        /// <param name="mustBeSamePage">Required to be on same page. Defaults to true.</param>
        public bool positionedOnSameLevel(Position pos1, Position pos2, bool mustBeSamePage = true)
        {
            return false;
        }

        /// <summary>
        /// Get the minimum distance between any two corners of two bounding boxes via the pythagorean formula.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        /// <param name="pageHeight">If you want to measure distance across pages, set the OCR page height otherwise locations on separate pages will raise an exception. Defaults to null.</param>
        public float getMinDistance(Position pos1, Position pos2, int pageHeight = null)
        {
            return 0f;
        }

        /// <summary>
        /// Get the amount of horizontal overlap between two bounding boxes.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        public float getHorizontalOverlap(Position pos1, Position pos2)
        {
            return 0f;
        }

        /// <summary>
        /// Get the amount of vertical overlap between two bounding boxes.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        public float getVerticalOverlap(Position pos1, Position pos2)
        {
            return 0f;
        }

        /// <summary>
        /// Get the vertical minimum distance between two bounding boxes.
        /// </summary>
        /// <param name="abovePos">The position expected to be above</param>
        /// <param name="belowPos">The position expected to be below</param>
        /// <param name="pageHeight">If you want to measure distances across pages, set the OCR page height otherwise locations on separate pages will raise an exception. Defaults to null.</param>
        private float getVerticalMinDistance(Position pos1, Position pos2, int pageHeight = null)
        {
            return 0f;
        }

        /// <summary>
        /// Get the horizontal minimum distance between two bounding boxes.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        private float getHorizontalMinDistance(Position pos1, Position pos2)
        {
            return 0f;
        }

        /// <summary>
        /// Get the distance between two points (x, y).
        /// </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        private float distanceBetweenPoints(Tuple<int, int> point1, Tuple<int, int> point2)
        {
            return 0f;
        }
        
        /// <summary>
        /// Get the manhattan distance between two points (x, y).
        /// </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        private float manhattanDistanceBetweenPoints(Tuple<int, int> point1, Tuple<int, int> point2)
        {
            return 0f;
        }

        /// <summary>
        /// Check whether two locations overlap on the yaxis, i.e. same "row".
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        private bool yAxisOverlap(Position pos1, Position pos2)
        {
            return false;
        }

        /// <summary>
        /// Check whether two locations overlap on the xaxis.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        private bool xAxisOverlap(Position pos1, Position pos2)
        {
            return false;
        }

        /// <summary>
        /// Check whether the first position is above the second.
        /// </summary>
        /// <param name="abovePos">The position excepted to be above</param>
        /// <param name="belowPos">The position excepted to be below</param>
        private bool yAxisAbove(Position abovePos, Position belowPos)
        {
            return false;
        }

        /// <summary>
        /// Check whether the two positions are on the same page.
        /// </summary>
        /// <param name="pos1">First position</param>
        /// <param name="pos2">Second position</param>
        private bool onSamePage(Position pos1, Position pos2)
        {
            return false;
        }
    }
}