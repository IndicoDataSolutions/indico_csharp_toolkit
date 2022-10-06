using System.Collections.Generic;

using IndicoToolkit.Types;

namespace IndicoToolkit.AutoReview
{
    /// <summary>
    /// Auto Review function delegate to pass review functions as arguments.
    /// </summary>
    public delegate List<Prediction> AutoReviewDelegate(List<Prediction> predictions, List<string> labels, float threshold);
    /// <summary>
    /// Review configuration for Auto Reviewer
    /// </summary>
    public class ReviewConfiguration
    {
        public List<FunctionConfig> FieldConfig { get; private set; }
        public Dictionary<string, AutoReviewDelegate> CustomFunctions { get; private set; }

        public ReviewConfiguration(
                List<FunctionConfig> fieldConfig,
                Dictionary<string, AutoReviewDelegate> customFunctions = default
            )
        {
            CustomFunctions = customFunctions == null ? new Dictionary<string, AutoReviewDelegate>() : customFunctions;
            FieldConfig = fieldConfig;
        }
    }
}