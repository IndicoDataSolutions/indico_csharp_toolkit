using System.Collections.Generic;

using IndicoToolkit.Types;

namespace IndicoToolkit.AutoReview
{
    /// <summary>
    /// Class for programatically reviewing workflow predictions
    /// <example>
    /// Example Usage:
    /// <code>
    /// AutoReviewer reviewer = new AutoReviewer(predictions, reviewConfig);
    /// reviewer.apply_review();
    /// </code>
    /// </example>
    /// <example>
    /// Get your updated predictions:
    /// <code>
    /// List<Prediction> updatedPredictions = reviewer.updatedPredictions;
    /// </code>
    /// </example>
    /// </summary>
    public class AutoReviewer
    {
        public List<FunctionConfig> FieldConfig { get; private set; }
        public Dictionary<string, AutoReviewDelegate> Reviewers { get; private set; }
        public List<Prediction> Predictions { get; private set; }
        public List<Prediction> UpdatedPredictions { get; private set; }

        public AutoReviewer(
            List<Prediction> predictions,
            ReviewConfiguration reviewConfig
        )
        {
            AutoReviewFunctions autoReviewFunctions = new AutoReviewFunctions();
            Reviewers = new Dictionary<string, AutoReviewDelegate>()
            {
                {"rejectByConfidence", autoReviewFunctions.rejectByConfidence},
                {"acceptByConfidence", autoReviewFunctions.acceptByConfidence},
                {"rejectByMinCharacterLength", autoReviewFunctions.rejectByMinCharacterLength},
                {"rejectByMaxCharacterLength", autoReviewFunctions.rejectByMaxCharacterLength}
            };
            FieldConfig = reviewConfig.FieldConfig;
            Reviewers = addReviewers(reviewConfig.CustomFunctions);
            Predictions = predictions;
            UpdatedPredictions = predictions;
        }

        /// <summary>
        /// Add custom functions into reviewers
        /// Overwrites any default reviwers if function names match
        /// </summary>
        public Dictionary<string, AutoReviewDelegate> addReviewers(Dictionary<string, AutoReviewDelegate> customFunctions)
        {
            foreach (KeyValuePair<string, AutoReviewDelegate> customFunction in customFunctions)
            {
                if (Reviewers.ContainsKey(customFunction.Key))
                {
                    Reviewers[customFunction.Key] = customFunction.Value;
                }
                else
                {
                    Reviewers.Add(customFunction.Key, customFunction.Value);
                }
            }
            return Reviewers;
        }

        public void applyReviews()
        {
            foreach (FunctionConfig functionConfig in FieldConfig)
            {
                string functionName = functionConfig.Function;
                try
                {
                    AutoReviewDelegate reviewFunction = Reviewers[functionName];
                    Kwargs kwargs = functionConfig.Kwargs;
                    UpdatedPredictions = reviewFunction(UpdatedPredictions, kwargs.Labels, kwargs.Threshold);
                }
                catch (KeyNotFoundException)
                {
                    throw new KeyNotFoundException($"{functionName} function was not found, did you specify it in FieldConfig?");
                }
            }
        }
    }
}