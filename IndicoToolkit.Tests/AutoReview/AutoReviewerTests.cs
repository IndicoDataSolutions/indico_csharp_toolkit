using Xunit;
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using IndicoToolkit.AutoReview;
using IndicoToolkit.Types;

namespace IndicoToolkit.Tests
{
    public class AutoReviewerTests
    {
        public AutoReviewerTests() { }

        [Fact]
        public void RejectByConfidence_MatchingLabelAboveThreshold_ShouldNotChange()
        {
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction() };
            ReviewConfiguration reviewConfig = new ReviewConfiguration();
            AutoReviewer autoReviewer = new AutoReviewer(predictions, reviewConfig);
        }


    }
}