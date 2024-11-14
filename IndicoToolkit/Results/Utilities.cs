using Newtonsoft.Json.Linq;
using System.Linq;

namespace IndicoToolkit.Results;


public static class Utils
{
    // Get the value of `key` from `json` if it exists and is of type `ValueType`.
    // Raise an exception otherwise.
    public static ValueType Get<ValueType>(JToken json, string key)
    {
        try
        {
            return json[key].ToObject<ValueType>();
        }
        catch (System.Exception)
        {
            throw new ResultException(
                $"JSON object `{json}` does not have a value for "
                + $"key `{key}` with type `{typeof(ValueType)}`"
            );
        }
    }

    // Determine whether `json` has `key` with a value of type `ValueType`.
    public static bool Has<ValueType>(JToken json, string key)
    {
        try
        {
            return json[key].ToObject<ValueType>() != null;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
