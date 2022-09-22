using System;
using System.Collections.Generic;

using IndicoToolkit.Types;

namespace IndicoToolkit.AutoReview
{
    /// <summary>
    /// 
    /// </summary>
    public class ReviewConfiguration
    {
        public List<FunctionConfig> FieldConfig { get; private set; }
        public Dictionary<string, Callable> CustomFunctions { get; private set; }

        public ReviewConfiguration(
                List<FunctionConfig> fieldConfig,
                Dictionary<string, Callable> customFunctions
            )
        {
            CustomFunctions = customFunctions;
            FieldConfig = fieldConfig;
        }
    }
}