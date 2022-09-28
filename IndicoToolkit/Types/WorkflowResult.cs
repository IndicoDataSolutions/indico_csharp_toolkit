
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using IndicoToolkit.Exception;

namespace IndicoToolkit.Types
{
    public class WorkflowResult
    {
        public JObject Result { get; set; }
        public string ModelName { get; set; }
        public string EtlUrl
        {
            get { return (string)Result.SelectToken("etl_output"); }
        }
        public JObject DocumentResults
        {
            get { return (JObject)Result.SelectToken("results.document.results"); }
        }
        public List<string> AvailableModelNames
        {
            get { return DocumentResults.Properties().Select(p => p.Name).ToList(); }
        }

        public string SubmissionId
        {
            get { return (string)Result.SelectToken("submission_id"); }
        }
        public string Errors
        {
            get { return (string)Result.SelectToken("errors"); }
        }
        public string ReviewId
        {
            get { return (string)Result.SelectToken("review_id"); }
        }
        public string ReviewerId
        {
            get { return (string)Result.SelectToken("reviewer_id"); }
        }
        public string ReviewNotes
        {
            get { return (string)Result.SelectToken("review_notes"); }
        }
        public string ReviewRejected
        {
            get { return (string)Result.SelectToken("review_rejected"); }
        }
        public bool AdminReview
        {
            get { return (bool)Result.SelectToken("admin_review"); }
        }

        public WorkflowResult(JObject result, string modelName)
        {
            Result = result;
            ModelName = modelName;
        }

        public List<Prediction> GetPredictions()
        {
            SetModelName();
            List<Prediction> predictions = new List<Prediction>();
            JArray results = (JArray)DocumentResults.SelectToken($"['{ModelName}']");
            if (results.SelectToken("pre_review") != null)
            {
                foreach (var predictionValue in results.SelectToken("pre_review"))
                {
                    Prediction prediction = new Prediction((JObject)predictionValue);
                    predictions.Add(prediction);
                }
            }
            else
            {
                foreach (var predictionValue in DocumentResults.SelectToken($"['{ModelName}']"))
                {
                    Prediction prediction = new Prediction((JObject)predictionValue);
                    predictions.Add(prediction);
                }
            }

            return predictions;
        }

        /// <summary>
        /// Attempts to select the relevant model name if not already specified.
        /// Raises error if multiple models are available and ModelName specified.
        /// </summary>
        private void SetModelName()
        {
            if (ModelName != null)
            {
                CheckIsValidModelName();
            }
            else if (AvailableModelNames.Count > 1)
            {
                throw new ToolkitInputException(
                    $"Multiple models available, you must set ModelName to one of {AvailableModelNames}"
                );
            }
            else
            {
                ModelName = AvailableModelNames[0];
            }
        }

        /// <summary>
        /// Check if ModelName exists in AvailableModelNames
        /// </summary>
        private void CheckIsValidModelName()
        {
            if (!AvailableModelNames.Contains(ModelName))
            {
                throw new ToolkitInputException(
                    $"{ModelName} is not an available model name. Options: {AvailableModelNames}"
                );
            }
        }
    }


}