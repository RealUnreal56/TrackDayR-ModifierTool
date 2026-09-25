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

            string url = CurrentVersionUrl + "?t=" + DateTime.UtcNow.Ticks;

            string onlineVersion = await client.GetStringAsync(url);

            onlineVersion = onlineVersion.Trim();

            if (!Version.TryParse(GetCurrentVersion(), out Version? currentVersion))
                return (false, onlineVersion);

            if (!Version.TryParse(onlineVersion, out Version? latestVersion))
                return (false, onlineVersion);

            return (latestVersion > currentVersion, onlineVersion);
        }
        catch
        {
            return (false, "");
        }
    }
}