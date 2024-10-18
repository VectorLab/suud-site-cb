
using System.Text.Json.Nodes;

namespace suud_site_cb.Utils;

public static class JsonUtil
{

    /* not support: float (use double instead) */
    public static T? GetPropertyTVal<T>(JsonObject jsonObject, string propertyName) where T : struct
    {
        try
        {
            if (jsonObject.TryGetPropertyValue(propertyName, out JsonNode? jsonNode))
            {
                if (jsonNode is JsonValue jsonValue)
                {
                    if (jsonValue.TryGetValue(out T? jsonValuePrimitive))
                    {
                        return jsonValuePrimitive;
                    }
                }
            }
        }
        catch { }
        return null;
    }

    public static T? GetPropertyTRef<T>(JsonObject jsonObject, string propertyName) where T : class
    {
        try
        {
            if (jsonObject.TryGetPropertyValue(propertyName, out JsonNode? jsonNode))
            {
                {
                    if (jsonNode is T jsonValue)
                    {// T is JsonNode
                        return jsonValue;
                    }
                }
                {
                    if (jsonNode is JsonValue jsonValue)
                    {
                        if (jsonValue.TryGetValue(out T? jsonValuePrimitive))
                        {
                            return jsonValuePrimitive;
                        }
                    }
                }
            }
        }
        catch { }
        return default;
    }

};
