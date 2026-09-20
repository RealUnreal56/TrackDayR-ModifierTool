using System.IO;

namespace ModifierTool;

public static class Config
{
    public static void Initialize()
    {
        string configPath = "config.txt";

        if (!File.Exists(configPath))
        {
            string configContent =
                "PATH = " + Environment.NewLine +
                "FULLSCREEN = false";

            File.WriteAllText(configPath, configContent);
            return;
        }

        string[] lines = File.ReadAllLines(configPath);
        List<string> configLines = lines.ToList();

        if (!lines.Any(line => line.StartsWith("PATH =")))
        {
            configLines.Add("PATH = ");
        }

        if (!lines.Any(line => line.StartsWith("FULLSCREEN =")))
        {
            configLines.Add("FULLSCREEN = false");
        }

        File.WriteAllLines(configPath, configLines);
    }
    public static void savePath(string path)
    {
        string[] lines = File.ReadAllLines("config.txt");
        lines[0] = "PATH = " + path;
        File.WriteAllLines("config.txt", lines);
    }

    public static string getPath()
    {
        string[] lines = File.ReadAllLines("config.txt");
        return lines[0].Substring(7); // Remove "PATH = " prefix
    }

    public static bool IsFullscreen()
    {
        string[] lines = File.ReadAllLines("config.txt");
        string fullscreenValue = lines[1].Substring(12); // Remove "FULLSCREEN = " prefix
        bool isFullscreen = bool.TryParse(fullscreenValue, out bool result) && result;
        return isFullscreen;
    }

    public static void SetFullscreen(bool isFullscreen)
    {
        string[] lines = File.ReadAllLines("config.txt");
        lines[1] = "FULLSCREEN = " + isFullscreen.ToString().ToLower();
        File.WriteAllLines("config.txt", lines);
    }
}