using System.Collections.Generic;

using IndicoToolkit.Types;

namespace IndicoToolkit.AutoReview
{
    /// <summary>
    /// Class that hosts suite of auto review functions
    /// </summary>
    public class AutoReviewFunctions
    {
        public AutoReviewFunctions() { }

        /// <summary>
        /// Rejects predictions below a given confidence threshold
        /// </summary>
        /// <param name="predictions">List of predictions</param>
        /// <param name="labels">List of labels</param>
        /// <param name="confThreshold">Threshold to reject predictions</param>
        /// <returns>
        /// all predictions 
        /// </returns>
        public List<Prediction> rejectByConfidence(List<Prediction> predictions, List<string> labels, float confThreshold = .5f)
        {
            foreach (Prediction prediction in predictions)
            {
                if (labels.Contains(prediction.Label))
                {
                    string predLabel = prediction.Label;
                    if (prediction.Confidence[predLabel] < confThreshold)
                    {
                        prediction.Rejected = true;
                        prediction.Accepted = false;
                    }
                }
            }
            return predictions;
        }

        /// <summary>
        /// Accepts predictions above a given confidence threshold
        /// </summary>
        /// <param name="predictions">List of predictions</param>
        /// <param name="labels">List of labels</param>
        /// <param name="confThreshold">Threshold to accept predictions</param>
        /// <returns>
        /// all predictions 
        /// </returns>
        public List<Prediction> acceptByConfidence(List<Prediction> predictions, List<string> labels, float confThreshold = .98f)
        {
            foreach (Prediction prediction in predictions)
            {
                if (labels.Contains(prediction.Label))
                {
                    string predLabel = prediction.Label;
                    if (prediction.Confidence[predLabel] > confThreshold)
                    {
                        prediction.Accepted = true;
                    }
                }
            }
            return predictions;
        }

        /// <summary>
        /// Rejects predictions shorter than a given minimum length
        /// </summary>
        /// <param name="predictions">List of predictions</param>
        /// <param name="labels">List of labels</param>
        /// <param name="minLengthThreshold">Threshold to reject predictions</param>
        /// <returns>
        /// all predictions 
        /// </returns>
        public List<Prediction> rejectByMinCharacterLength(List<Prediction> predictions, List<string> labels, float minLengthThreshold = 3)
        {
            foreach (Prediction prediction in predictions)
            {
                if (labels.Contains(prediction.Label))
                {
                    string textValue = prediction.Text;
                    if (textValue.Length < minLengthThreshold)
                    {
                        prediction.Rejected = true;
                    }
                }
            }
            return predictions;
        }

        /// <summary>
        /// Rejects predictions longer than a given maximum length
        /// </summary>
        /// <param name="predictions">List of predictions</param>
        /// <param name="labels">List of labels</param>
        /// <param name="maxLengthThreshold">Threshold to reject predictions</param>
        /// <returns>
        /// all predictions 
        /// </returns>
        public List<Prediction> rejectByMaxCharacterLength(List<Prediction> predictions, List<string> labels, float maxLengthThreshold = 10)
        {
            foreach (Prediction prediction in predictions)
            {
                if (labels.Contains(prediction.Label))
                {
                    string textValue = prediction.Text;
                    if (textValue.Length > maxLengthThreshold)
                    {
                        prediction.Rejected = true;
                    }
                }
            }
            return predictions;
        }
    }
}