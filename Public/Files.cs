using System.IO;

namespace TrackDayRModifier;

public static class Files
{
    public static DirectoryInfo[] loadBikes()
    {
        string path = Config.getPath() + "\\bikes";

        DirectoryInfo[] bikeFolders = new DirectoryInfo[0];

        foreach (string folder in Directory.GetDirectories(path))
        {
            Array.Resize(ref bikeFolders, bikeFolders.Length + 1);
            bikeFolders[bikeFolders.Length - 1] = new DirectoryInfo(folder);
        }

        return bikeFolders;
    }

    public static FileInfo loadBike(string bikeFileName)
    {
        string path = Config.getPath() + "\\bikes\\" + bikeFileName;

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file '{bikeFileName}' does not exist in the bikes directory.");
        }

        return new FileInfo(path);
    }
}