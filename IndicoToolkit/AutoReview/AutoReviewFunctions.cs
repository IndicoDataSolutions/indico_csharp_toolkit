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
                if (prediction.getValue("rejected") == null && labels.Contains(prediction.getLabel()))
                {
                    string predLabel = prediction.getLabel();
                    if (prediction.getValue($"confidence.{predLabel}") < confThreshold)
                    {
                        prediction.setValue("rejected", true);
                        prediction.removeKey("accepted");
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
                if (prediction.getValue("rejected") == null && labels.Contains(prediction.getLabel()))
                {
                    string predLabel = prediction.getLabel();
                    if (prediction.getValue($"confidence.{predLabel}") > confThreshold)
                    {
                        prediction.setValue("accepted", true);
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
                if (labels.Contains(prediction.getLabel()))
                {
                    string textValue = prediction.getValue("text");
                    if (textValue.Length < minLengthThreshold)
                    {
                        prediction.setValue("rejected", true);
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
                if (labels.Contains(prediction.getLabel()))
                {
                    string textValue = prediction.getValue("text");
                    if (textValue.Length > maxLengthThreshold)
                    {
                        prediction.setValue("rejected", true);
                    }
                }
            }
            return predictions;
        }
    }
}