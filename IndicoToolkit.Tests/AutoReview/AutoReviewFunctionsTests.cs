using Xunit;
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using IndicoToolkit.AutoReview;
using IndicoToolkit.Types;

namespace IndicoToolkit.Tests
{
    public class AutoReviewFunctionsTests
    {
        public AutoReviewFunctionsTests() { }

        [Fact]
        public void RejectByConfidence_MatchingLabelAboveThreshold_ShouldNotChange()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction() };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.rejectByConfidence(predictions, labels);
            Assert.Null(resultPredictions[0].getValue("rejected"));
        }

        [Fact]
        public void RejectByConfidence_MatchingLabelBelowThreshold_ShouldReject()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            JObject newConfidence = Utils.CreateNewConfidence("testLabel", .3f);
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction(confidence: newConfidence) };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.rejectByConfidence(predictions, labels);
            bool resultIsRejected = resultPredictions[0].getValue("rejected");
            Assert.True(resultIsRejected);
        }

        [Fact]
        public void AcceptByConfidence_MatchingLabelAboveThreshold_ShouldNotChange()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction() };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.acceptByConfidence(predictions, labels);
            Assert.Null(resultPredictions[0].getValue("accepted"));
        }

        [Fact]
        public void AcceptByConfidence_LoweringConfThreshold_ShouldAccept()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction() };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.acceptByConfidence(predictions, labels, confThreshold: .2f);
            bool resultIsAccepted = resultPredictions[0].getValue("accepted");
            Assert.True(resultIsAccepted);
        }

        [Fact]
        public void AcceptByConfidence_MatchingLabelBelowThreshold_ShouldAccept()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            JObject newConfidence = Utils.CreateNewConfidence("testLabel", .99f);
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction(confidence: newConfidence) };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.acceptByConfidence(predictions, labels);
            bool resultIsAccepted = resultPredictions[0].getValue("accepted");
            Assert.True(resultIsAccepted);
        }

        [Fact]
        public void RejectByMinCharacterLength_MatchingLabelAboveThreshold_ShouldNotChange()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction(text: "hello") };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.rejectByMinCharacterLength(predictions, labels);
            Assert.Null(resultPredictions[0].getValue("rejected"));
        }

        [Fact]
        public void RejectByMinCharacterLength_LoweringThreshold_ShouldReject()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction(text: "hello") };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.rejectByMinCharacterLength(predictions, labels, minLengthThreshold: 6);
            bool resultIsRejected = resultPredictions[0].getValue("rejected");
            Assert.True(resultIsRejected);
        }

        [Fact]
        public void RejectByMaxCharacterLength_MatchingLabelAboveThreshold_ShouldNotChange()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction(text: "hello") };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.rejectByMaxCharacterLength(predictions, labels);
            Assert.Null(resultPredictions[0].getValue("rejected"));
        }

        [Fact]
        public void RejectByMaxCharacterLength_LoweringThreshold_ShouldReject()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction(text: "hello") };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.rejectByMaxCharacterLength(predictions, labels, maxLengthThreshold: 4);
            bool resultIsRejected = resultPredictions[0].getValue("rejected");
            Assert.True(resultIsRejected);
        }
    }
}