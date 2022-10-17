using Xunit;
using System;
using System.Collections.Generic;

using IndicoToolkit.AutoReview;
using IndicoToolkit.Types;

namespace IndicoToolkit.Tests
{
    public class AutoReviewerFixture : IDisposable
    {
        public ReviewConfiguration BasicReviewConfig { get; private set; }
        public ReviewConfiguration NewFunctionReviewConfig { get; private set; }
        public ReviewConfiguration ManyFunctionsReviewConfig { get; private set; }

        public AutoReviewerFixture()
        {
            Dictionary<string, AutoReviewDelegate> customFunctions = new Dictionary<string, AutoReviewDelegate>();
            BasicReviewConfig = GetBasicReviewConfig();
            NewFunctionReviewConfig = GetNewFunctionReviewConfig();
            ManyFunctionsReviewConfig = GetManyFunctionsReviewConfig();
        }

        public ReviewConfiguration GetBasicReviewConfig()
        {
            List<FunctionConfig> fieldConfig = new List<FunctionConfig>()
            {
                new FunctionConfig
                (
                    "rejectByConfidence",
                    new Kwargs
                    (
                        labels: new List<string>() { "testLabel" },
                        threshold: .99f
                    )
                ),
            };
            return new ReviewConfiguration(fieldConfig);
        }

        public ReviewConfiguration GetNewFunctionReviewConfig()
        {
            List<FunctionConfig> fieldConfig = new List<FunctionConfig>()
            {
                new FunctionConfig
                (
                    "newReviewFunction",
                    new Kwargs
                    (
                        labels: new List<string>() { "testLabel" },
                        threshold: 5f
                    )
                ),
            };

            Dictionary<string, AutoReviewDelegate> customFunctions = new Dictionary<string, AutoReviewDelegate>()
            {
                { "newReviewFunction", newReviewFunction }
            };
            return new ReviewConfiguration(fieldConfig, customFunctions);
        }

        public ReviewConfiguration GetManyFunctionsReviewConfig()
        {
            List<FunctionConfig> fieldConfig = new List<FunctionConfig>()
            {
                new FunctionConfig(
                    "rejectByConfidence",
                    new Kwargs
                    (
                        labels: new List<string>() { "testLabel" },
                        threshold: .99f
                    )
                ),
                new FunctionConfig(
                    "rejectByMaxCharacterLength",
                    new Kwargs
                    (
                        labels: new List<string>() { "newLabel" },
                        threshold: 10f
                    )
                )
            };
            return new ReviewConfiguration(fieldConfig);
        }

        public static List<Prediction> newReviewFunction(List<Prediction> predictions, List<string> labels, float threshold)
        {
            return new List<Prediction>();
        }

        public void Dispose()
        {
            // ... clean up test data if needed
        }

    }
    public class AutoReviewerTests : IClassFixture<AutoReviewerFixture>
    {
        AutoReviewerFixture Fixture { get; }

        public AutoReviewerTests(AutoReviewerFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public void AutoReviewer_OneFunction_ShouldReject()
        {
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction() };
            ReviewConfiguration reviewConfig = Fixture.BasicReviewConfig;
            AutoReviewer autoReviewer = new AutoReviewer(predictions, reviewConfig);
            autoReviewer.applyReviews();
            bool resultIsRejected = autoReviewer.UpdatedPredictions[0].getValue("rejected");
            Assert.True(resultIsRejected);
        }

        [Fact]
        public void AutoReviewer_NewFunction_ShouldNotThrowException()
        {
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction() };
            ReviewConfiguration reviewConfig = Fixture.NewFunctionReviewConfig;
            AutoReviewer autoReviewer = new AutoReviewer(predictions, reviewConfig);
            autoReviewer.applyReviews();
            Assert.Equal(autoReviewer.UpdatedPredictions.Count, 0);
        }

        [Fact]
        public void AutoReviewer_ManyFunctions_ShouldRejectOne()
        {
            List<Prediction> predictions = new List<Prediction>()
            {
                Utils.CreatePrediction(),
                Utils.CreatePrediction(label: "newLabel"),
            };
            ReviewConfiguration reviewConfig = Fixture.ManyFunctionsReviewConfig;
            AutoReviewer autoReviewer = new AutoReviewer(predictions, reviewConfig);
            autoReviewer.applyReviews();
            bool resultOneIsRejected = autoReviewer.UpdatedPredictions[0].getValue("rejected");
            Assert.True(resultOneIsRejected);
            var resultTwoIsUnchanged = autoReviewer.UpdatedPredictions[1].getValue("rejected");
            Assert.Null(resultTwoIsUnchanged);
        }

    }
}