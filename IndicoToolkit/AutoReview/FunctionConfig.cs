using System;
using System.Collections.Generic;

namespace IndicoToolkit.AutoReview
{
    public class FunctionConfig
    {
        public string Function { get; private set; }
        public Tuple<List<string>, float> Kwargs { get; private set; }

        public FunctionConfig(
            string function,
            Tuple<List<string>, float> kwargs
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
            return Kwargs.Item1;
        }

        /// <summary>
        /// Get confidence threshold from kwargs.
        /// </summary>
        public float getConfThreshold()
        {
            return Kwargs.Item2;
        }

        /// <summary>
        /// Add new label to labels in kwargs.
        /// </summary>
        public void addLabel(string label)
        {
            Kwargs.Item1.Add(label);
        }
    }
}