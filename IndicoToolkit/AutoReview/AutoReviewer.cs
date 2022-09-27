using System;
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
        public List<> Reviewers { get; private set; }
        public List<Prediction> Predictions { get; private set; }
        public List<Prediction> UpdatedPredictions { get; private set; }

        public AutoReviewer(
            List<Prediction> predictions,
            ReviewConfiguration reviewConfig
        )
        {
            FieldConfig = reviewConfig.FieldConfig;
            Reviewers = addReviewers(reviewConfig.CustomFunctions);
            Predictions = predictions;
            UpdatedPredictions = predictions;
        }

        /// <summary>
        /// Add custom functions into reviewers
        /// Overwrites any default reviwers if function names match
        /// </summary>
        public Dictionary<string, Delegate> addReviewers()
        {
            foreach ()
            {

            }
            return;
        }

    }
}