using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using IndicoToolkit.Types;

namespace IndicoToolkit.Association
{
    /// <summary>
    /// Class <c>Association</c> is the base class for matching tokens to extraction predictions.
    /// </summary>
    abstract class Association
    {
        public List<Prediction> Predictions { get; private set; }
        public List<Position> MappedPositions { get; private set; }
        public List<Prediction> ManuallyAddedPreds { get; private set; }
        public List<Prediction> ErroredPredictions { get; private set; }
        public Association(List<Prediction> predictions = null)
        {
            Predictions = (predictions == null ? new List<Prediction>() : predictions);
            MappedPositions = new List<Position>();
            ManuallyAddedPreds = new List<Prediction>();
            ErroredPredictions = new List<Prediction>();
        }

        public abstract void matchPredToToken();

        /// <summary>
        /// Sorts predictions by start index.
        /// </summary>
        public List<Prediction> sortPredictionsByStartIndex(List<Prediction> predictions)
        {
            return predictions.OrderBy(pred => pred.getValue("start")).ToList();
        }

        /// <summary>
        /// Return mapped positions by page on which they first appear
        /// </summary>
        public Dictionary<int, List<Position>> mappedPositionsByPage()
        {
            ConcurrentDictionary<int, List<Position>> pageMap = new ConcurrentDictionary<int, List<Position>>();
            foreach (Position position in MappedPositions)
            {
                pageMap[position.pageNum].Add(position);
            }
            return new Dictionary<int, List<Position>>(pageMap);
        }

        public bool isManuallyAddedPred(Prediction prediction)
        {
            return Extractions.isManuallyAddedPrediction(prediction);
        }

        /// <summary>
        /// Boolean return value indicates whether or not sequences overlap.
        /// </summary>
        public bool sequencesOverlap(Prediction x, Prediction y)
        {
            return x.getValue("start") < y.getValue("end") && y.getValue("start") < x.getValue("end");
        }

        /// <summary>
        /// Boolean return value indicates whether or not sequences are exact.
        /// </summary>
        public bool sequencesExact(Prediction x, Prediction y)
        {
            return x.getValue("start") == y.getValue("start") && x.getValue("end") == y.getValue("end");
        }

        internal void checkIfTokenMatchFound(Prediction pred, bool noMatchIndicator)
        {
            if (noMatchIndicator)
            {
                pred.setValue("error", "No matching token found for extraction");
                throw new Exception($"Couldn't match a token to this prediction:\n{pred}");
            }
        }
    }
}