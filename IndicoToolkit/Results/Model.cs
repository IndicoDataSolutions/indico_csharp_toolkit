using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace IndicoToolkit.Results;


public enum TaskType
{
    CLASSIFICATION,
    DOCUMENT_EXTRACTION,
    FORM_EXTRACTION,
    UNBUNDLING
}


public class ModelGroup : PrettyPrint
{
    public int? Id { get; init; }  // v1 result files don't include Model Group IDs.
    public string Name { get; init; }
    public TaskType TaskType { get; init; }

    // Determine the task type of a model from its string representation.
    public static TaskType TaskTypeFromString(string taskType)
    {
        if (taskType == "classification")
            return TaskType.CLASSIFICATION;
        else if (taskType == "annotation")
            return TaskType.DOCUMENT_EXTRACTION;
        else if (taskType == "form_extraction")
            return TaskType.FORM_EXTRACTION;
        else if (taskType == "classification_unbundling")
            return TaskType.UNBUNDLING;
        else
            throw new ResultException($"unsupported task type `{taskType}`");
    }

    // Determine the task type of a model using a heuristic on its pre-review structure,
    // (whether it's an object or an array) and its first prediction (the keys it has).
    public static TaskType TaskTypeFromV1Heuristic(JToken preReviewJson)
    {
        if (preReviewJson.HasValues)  // Extraction model sections may be empty.
        {
            var prediction = preReviewJson.First;

            if (Utils.Has<string>(prediction, "type"))
                return TaskType.FORM_EXTRACTION;
            else if (Utils.Has<string>(prediction, "text"))
                return TaskType.DOCUMENT_EXTRACTION;
            else
                return TaskType.CLASSIFICATION;
        }
        else
        {
            return TaskType.DOCUMENT_EXTRACTION;
        }
    }

    // Create a ModelGroup from a Model/Predictions key/value pair of a
    // `["results"]["document"]["results"]` section of a v1 result file.
    public static ModelGroup FromV1Json(KeyValuePair<string, JToken> modelPredictions)
    {
        return new ModelGroup
        {
            Id = null,
            Name = modelPredictions.Key,
            TaskType = TaskTypeFromV1Heuristic(
                Utils.Get<JToken>(modelPredictions.Value, "pre_review")
            ),
        };
    }

    // Create a ModelGroup from a v3 `modelgroup_metadata` list item.
    public static ModelGroup FromV3Json(JToken json)
    {
        return new ModelGroup
        {
            Id = Utils.Get<int>(json, "id"),
            Name = Utils.Get<string>(json, "name"),
            TaskType = TaskTypeFromString(Utils.Get<string>(json, "task_type")),
        };
    }
}
