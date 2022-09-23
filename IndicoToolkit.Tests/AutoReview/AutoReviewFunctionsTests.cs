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
        public void EmptyOrMatchingLabel_EmptyLabelsList_ShouldBeTrue()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            Prediction prediction = Utils.CreatePrediction();
            List<string> emptyLabelsList = new List<string>();
            bool result = reviewFunctions.emptyOrMatchingLabel(prediction, emptyLabelsList);
            Assert.True(result);
        }

        [Fact]
        public void EmptyOrMatchingLabel_MatchingLabel_ShouldBeTrue()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            Prediction prediction = Utils.CreatePrediction();
            List<string> labels = new List<string>() { "dummyLabel", "testLabel" };
            bool result = reviewFunctions.emptyOrMatchingLabel(prediction, labels);
            Assert.True(result);
        }

        [Fact]
        public void EmptyOrMatchingLabel_NoMatchingLabel_ShouldBeFalse()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            Prediction prediction = Utils.CreatePrediction();
            List<string> labels = new List<string>() { "dummyLabel" };
            bool result = reviewFunctions.emptyOrMatchingLabel(prediction, labels);
            Assert.False(result);
        }

        [Fact]
        public void RejectByConfidence_Default_ShouldNotChange()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction() };
            List<Prediction> resultPredictions = reviewFunctions.rejectByConfidence(predictions);
            Assert.Null(resultPredictions[0].getValue("rejected"));
        }

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
        public void RemoveByConfidence_Default_ShouldNotChange()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction() };
            List<Prediction> resultPredictions = reviewFunctions.removeByConfidence(predictions);
            Assert.Equal(resultPredictions.Count, 1);
        }

        [Fact]
        public void RemoveByConfidence_MatchingLabelAboveThreshold_ShouldNotChange()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction() };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.removeByConfidence(predictions, labels);
            Assert.Equal(resultPredictions.Count, 1);
        }

        [Fact]
        public void RemoveByConfidence_MatchingLabelBelowThreshold_ShouldRemove()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            JObject newConfidence = Utils.CreateNewConfidence("testLabel", .3f);
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction(confidence: newConfidence) };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.removeByConfidence(predictions, labels);
            Assert.Equal(resultPredictions.Count, 0);
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
        public void AcceptByAllMatchAndConfidence_SameLabelDifferentValues_ShouldNotChange()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction(), Utils.CreatePrediction(text: "different text") };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.acceptByAllMatchAndConfidence(predictions, labels, confThreshold: .4f);
            Assert.Null(resultPredictions[0].getValue("accepted"));
            Assert.Null(resultPredictions[1].getValue("accepted"));
        }

        [Fact]
        public void AcceptByAllMatchAndConfidence_LowerThreshold_ShouldAcceptSingle()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            JObject newConfidence = Utils.CreateNewConfidence("anotherLabel", .4f);
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction(text: "not this", label: "anotherLabel", confidence: newConfidence), Utils.CreatePrediction(text: "this") };
            List<string> labels = new List<string>() { "anotherLabel", "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.acceptByAllMatchAndConfidence(predictions, labels, confThreshold: .5f);
            Assert.True(resultPredictions[0].getValue("accepted") == null && resultPredictions[1].getValue("accepted") == true);
        }

        [Fact]
        public void AcceptByAllMatchAndConfidence_LowerThreshold_ShouldAcceptMultiple()
        {
            AutoReviewFunctions reviewFunctions = new AutoReviewFunctions();
            List<Prediction> predictions = new List<Prediction>() { Utils.CreatePrediction(text: "hello"), Utils.CreatePrediction(text: "hello") };
            List<string> labels = new List<string>() { "testLabel" };
            List<Prediction> resultPredictions = reviewFunctions.acceptByAllMatchAndConfidence(predictions, labels, confThreshold: .3f);
            bool result = (bool)resultPredictions[0].getValue("accepted") && (bool)resultPredictions[1].getValue("accepted");
            Assert.True(result);
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