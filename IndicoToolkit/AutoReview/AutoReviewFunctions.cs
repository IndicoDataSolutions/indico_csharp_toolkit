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
        /// Determine whether prediction label matches a label in labels or labels list is empty
        /// </summary>
        /// <param name="prediction">prediction to check label</param>
        /// <param name="labels">List of labels</param>
        /// <returns>
        /// true labels list is empty or prediction label matches a label in the list
        /// </returns>
        public bool emptyOrMatchingLabel(Prediction prediction, List<string> labels)
        {
            if (labels == null || labels.Count == 0 || labels.Contains(prediction.getLabel()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Rejects predictions below a given confidence threshold
        /// </summary>
        /// <param name="predictions">List of predictions</param>
        /// <param name="labels">List of labels</param>
        /// <param name="confThreshold">Threshold to reject predictions</param>
        /// <returns>
        /// all predictions 
        /// </returns>
        public List<Prediction> rejectByConfidence(List<Prediction> predictions, List<string> labels = default, float confThreshold = .5f)
        {
            foreach (Prediction prediction in predictions)
            {
                if (prediction.getValue("rejected") == null)
                {
                    if (!emptyOrMatchingLabel(prediction, labels))
                    {
                        continue;
                    }
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
        /// Removes predictions below a given confidence threshold
        /// </summary>
        /// <param name="predictions">List of predictions</param>
        /// <param name="labels">List of labels</param>
        /// <param name="confThreshold">Threshold to remove predictions</param>
        /// <returns>
        /// all predictions above confThreshold. 
        /// </returns>
        public List<Prediction> removeByConfidence(List<Prediction> predictions, List<string> labels = default, float confThreshold = .5f)
        {
            List<Prediction> predictionsToRemove = new List<Prediction>();
            foreach (Prediction prediction in predictions)
            {
                if (prediction.getValue("rejected") == null)
                {
                    if (!emptyOrMatchingLabel(prediction, labels))
                    {
                        continue;
                    }
                    string predLabel = prediction.getLabel();
                    if (prediction.getValue($"confidence.{predLabel}") < confThreshold)
                    {
                        predictionsToRemove.Add(prediction);
                    }
                }
            }
            predictions.RemoveAll(prediction => predictionsToRemove.Contains(prediction));
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
        public List<Prediction> acceptByConfidence(List<Prediction> predictions, List<string> labels = default, float confThreshold = .98f)
        {
            foreach (Prediction prediction in predictions)
            {
                if (prediction.getValue("rejected") == null)
                {
                    if (!emptyOrMatchingLabel(prediction, labels))
                    {
                        continue;
                    }
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
        /// Accepts all predictions for a class if all their values are the same, and all confidence scores are above a given confidence threshold
        /// </summary>
        /// <param name="predictions">List of predictions</param>
        /// <param name="labels">List of labels</param>
        /// <param name="confThreshold">Threshold to accept predictions</param>
        /// <returns>
        /// all predictions 
        /// </returns>
        public List<Prediction> acceptByAllMatchAndConfidence(List<Prediction> predictions, List<string> labels = default, float confThreshold = .98f)
        {
            Dictionary<string, HashSet<string>> predictionMap = new Dictionary<string, HashSet<string>>();
            foreach (Prediction prediction in predictions)
            {
                if (prediction.getValue("rejected") == null)
                {
                    if (!emptyOrMatchingLabel(prediction, labels))
                    {
                        continue;
                    }
                    string predLabel = prediction.getLabel();
                    if (prediction.getValue($"confidence.{predLabel}") > confThreshold)
                    {
                        if (predictionMap.TryGetValue(predLabel, out var temp))
                        {
                            predictionMap[predLabel].Add((string)prediction.getValue("text"));
                        }
                        else
                        {
                            predictionMap[predLabel] = new HashSet<string>() { (string)prediction.getValue("text") };
                        }
                    }
                    else
                    {
                        predictionMap[predLabel] = new HashSet<string>() { "~", "~~" };
                    }
                }
            }
            foreach (Prediction prediction in predictions)
            {
                if (predictionMap.ContainsKey(prediction.getLabel()))
                {
                    if (predictionMap[prediction.getLabel()].Count == 1)
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
        public List<Prediction> rejectByMinCharacterLength(List<Prediction> predictions, List<string> labels = default, float minLengthThreshold = 3)
        {
            foreach (Prediction prediction in predictions)
            {
                if (emptyOrMatchingLabel(prediction, labels))
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
        public List<Prediction> rejectByMaxCharacterLength(List<Prediction> predictions, List<string> labels = default, float maxLengthThreshold = 10)
        {
            foreach (Prediction prediction in predictions)
            {
                if (emptyOrMatchingLabel(prediction, labels))
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