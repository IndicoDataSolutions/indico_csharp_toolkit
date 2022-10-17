using System.Collections.Generic;

namespace IndicoToolkit.AutoReview
{
    public class Kwargs
    {
        public List<string> Labels { get; set; }
        public float Threshold { get; set; }

        public Kwargs(
            List<string> labels,
            float threshold
        )
        {
            Labels = labels;
            Threshold = threshold;
        }
    }

    public class FunctionConfig
    {
        public string Function { get; private set; }
        public Kwargs Kwargs { get; private set; }

        public FunctionConfig(
            string function,
            Kwargs kwargs
        )
        {
            Function = function;
            Kwargs = kwargs;
        }

        /// <summary>
        /// Get labels from kwargs.
        /// </summary>
        public List<string> getLabels()
        {
            return Kwargs.Labels;
        }

        /// <summary>
        /// Get confidence threshold from kwargs.
        /// </summary>
        public float getThreshold()
        {
            return Kwargs.Threshold;
        }

        /// <summary>
        /// Add new label to labels in kwargs.
        /// </summary>
        public void addLabel(string label)
        {
            Kwargs.Labels.Add(label);
        }
    }
}