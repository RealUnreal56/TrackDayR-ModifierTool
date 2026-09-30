using System.Text.Json;
using System.IO;

namespace ModifierTool;

public static class Config
{
    private static readonly string ConfigFolder =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ModifierTool"
        );

    private static readonly string ConfigPath =
        Path.Combine(ConfigFolder, "config.json");

    private class ConfigData
    {
        public string PATH_MOD { get; set; } = "";

        public bool FULLSCREEN { get; set; } = false;
    }

    public static void Initialize()
    {
        if (!Directory.Exists(ConfigFolder))
        {
            Directory.CreateDirectory(ConfigFolder);
        }

        if (!File.Exists(ConfigPath))
        {
            ConfigData config = new ConfigData();

            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(ConfigPath, json);
        }
    }

    public static void saveModPath(string path)
    {
        ConfigData config = Load();

        config.PATH_MOD = path;

        Save(config);
    }

    public static string getModPath()
    {
        ConfigData config = Load();

        return config.PATH_MOD;
    }

    public static bool IsFullscreen()
    {
        ConfigData config = Load();

        return config.FULLSCREEN;
    }

    public static void SetFullscreen(bool isFullscreen)
    {
        ConfigData config = Load();

        config.FULLSCREEN = isFullscreen;

        Save(config);
    }

    private static ConfigData Load()
    {
        if (!File.Exists(ConfigPath))
        {
            return new ConfigData();
        }

        string json = File.ReadAllText(ConfigPath);

        return JsonSerializer.Deserialize<ConfigData>(json)
               ?? new ConfigData();
    }

    private static void Save(ConfigData config)
    {
        string json = JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(ConfigPath, json);
    }
}