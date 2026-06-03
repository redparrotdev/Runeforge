namespace Sandbox.Scenes.TurnBasedCombatSample.Data;

internal static class ParametersExtensions
{
    public static string GetString(this Parameters parameters, string key, string defaultValue)
    {
        if (parameters.TryGetValue(key, out var value))
        {
            return value;
        }

        return defaultValue;
    }

    public static int GetInt(this Parameters parameters, string key, int defaultValue)
    {
        if (parameters.TryGetValue(key, out var value) && int.TryParse(value, out var intValue))
        {
            return intValue;
        }
        return defaultValue;
    }

    public static float GetFloat(this Parameters parameters, string key, float defaultValue)
    {
        if (parameters.TryGetValue(key, out var value) && float.TryParse(value, out var floatValue))
        {
            return floatValue;
        }
        return defaultValue;
    }

    public static bool GetBool(this Parameters parameters, string key, bool defaultValue)
    {
        if (parameters.TryGetValue(key, out var value) && bool.TryParse(value, out var boolValue))
        {
            return boolValue;
        }
        return defaultValue;
    }
}
