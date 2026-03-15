using System.Text.Json;

namespace AtlasPacker.Helpers;

internal static class ConfigHelper
{
    private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        IndentSize = 2
    };

    public static bool ConfigFileExists()
    {
        return File.Exists(Constants.CONFIG_FILE_NAME);
    }

    public static void CreateConfigFile(AtlasInfo info)
    {
        using var fs = File.OpenWrite(Constants.CONFIG_FILE_NAME);
        using var writer = new StreamWriter(fs);

        var json = JsonSerializer.Serialize(info, _serializerOptions);

        writer.Write(json);
        writer.Flush();
    }

    public static AtlasInfo GetConfig()
    {
        using var fs = File.OpenRead(Constants.CONFIG_FILE_NAME);

        var config = JsonSerializer.Deserialize<AtlasInfo>(fs, _serializerOptions)
                     ?? throw new FileNotFoundException("Missing config.json file. Call initialize before using any other commands.");

        return config;
    }
}
