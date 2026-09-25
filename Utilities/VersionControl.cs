using System.Net.Http;
using System.Windows;

namespace ModifierTool;

public static class VersionControl
{
    private const string CurrentVersionUrl =
        "https://raw.githubusercontent.com/RealUnreal56/TrackDayR-ModifierTool/main/currentVersion.txt";

    public static string GetCurrentVersion()
    {
        return "1.0.0";
    }

    public static async Task<(bool UpdateAvailable, string OnlineVersion)> CheckForUpdates()
    {
        try
        {
            using HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.UserAgent.ParseAdd("ModifierTool");

            string onlineVersion = await client.GetStringAsync(CurrentVersionUrl);
            onlineVersion = onlineVersion.Trim();

            string currentVersion = GetCurrentVersion();

            if (!Version.TryParse(currentVersion, out Version? current))
                return (false, onlineVersion);

            if (!Version.TryParse(onlineVersion, out Version? online))
                return (false, onlineVersion);

            return (online > current, onlineVersion);
        }
        catch (Exception ex)
        {

            return (false, "");
        }
    }
}