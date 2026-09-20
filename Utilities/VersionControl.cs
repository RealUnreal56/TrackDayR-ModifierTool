using System.Net.Http;
using System.Text.Json;

namespace TrackDayRModifier;

public static class VersionControl
{
    private const string LatestReleaseUrl =
        "https://api.github.com/repos/RealUnreal56/TrackDayR-Modifier/releases/latest";

    public static string GetCurrentVersion()
    {
        return "1.0.0";
    }

    public static async Task<(bool UpdateAvailable, string OnlineVersion)> CheckForUpdates()
    {
        try
        {
            using HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.UserAgent.ParseAdd("TrackDayR-Modifier");

            string json = await client.GetStringAsync(LatestReleaseUrl);

            using JsonDocument document = JsonDocument.Parse(json);

            string onlineVersion = document.RootElement
                .GetProperty("tag_name")
                .GetString() ?? "";

            onlineVersion = onlineVersion.TrimStart('v', 'V');

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