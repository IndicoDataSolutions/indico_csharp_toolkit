using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using IndicoToolkit.Exception;
using IndicoToolkit.Types;

namespace IndicoToolkit.Association
{
    /// <summary>
    /// Class <c>Association</c> is the base class for matching tokens to extraction predictions.
    /// </summary>
    public abstract class Association
    {
        public List<Prediction> Predictions { get; set; }
        public List<MappedPrediction> MappedPositions { get; set; }
        public List<Prediction> ManuallyAddedPreds { get; set; }
        public List<Prediction> ErroredPredictions { get; set; }
        public Association(List<Prediction> predictions = null)
        {
            Predictions = (predictions == null ? new List<Prediction>() : predictions);
            MappedPositions = new List<MappedPrediction>();
            ManuallyAddedPreds = new List<Prediction>();
            ErroredPredictions = new List<Prediction>();
        }

        public abstract int matchPredToToken(Prediction pred, List<OcrToken> OcrTokens, int predIndex);

        /// <summary>
        /// Sorts predictions by start index.
        /// </summary>
        public List<Prediction> sortPredictionsByStartIndex(List<Prediction> predictions)
        {
            return predictions.OrderBy(pred => pred.Start).ToList();
        }

        /// <summary>
        /// Return mapped positions by page on which they first appear
        /// </summary>
        public Dictionary<int, List<MappedPrediction>> mappedPositionsByPage()
        {
            ConcurrentDictionary<int, List<MappedPrediction>> pageMap = new ConcurrentDictionary<int, List<MappedPrediction>>();
            foreach (MappedPrediction position in MappedPositions)
            {
                pageMap[position.PageNum].Add(position);
            }
            return new Dictionary<int, List<MappedPrediction>>(pageMap);
        }

        public bool isManuallyAddedPred(Prediction prediction)
        {
            return Extractions.isManuallyAddedPrediction(prediction);
        }

        /// <summary>
        /// Boolean return value indicates whether or not sequences overlap.
        /// </summary>
        public bool sequencesOverlap(DocOffset x, Prediction y)
        {
            return x.Start < y.End && y.Start < x.End;
        }

        /// <summary>
        /// Boolean return value indicates whether or not sequences are exact.
        /// </summary>
        public bool sequencesExact(Prediction x, Prediction y)
        {
            return x.Start == y.Start && x.End == y.End;
        }

        internal void checkIfTokenMatchFound(Prediction pred, bool noMatchIndicator)
        {
            if (noMatchIndicator)
            {
                throw new ToolkitInputException($"Couldn't match a token to this prediction:\n{pred}");
            }
        }
    }
}