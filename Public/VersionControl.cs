using System.Net.Http;
using System.Windows;

namespace TrackDayRModifier;

public static class VersionControl
{
    private const string VersionUrl =
        "https://drive.google.com/uc?export=download&id=12ql2kj9Wo3btpyUtiSp537xewy8M0s9z";

    public static string GetCurrentVersion()
    {
        return "1.3";
    }

    public static async Task<(bool UpdateAvailable, string OnlineVersion)> CheckForUpdates()
    {
        try
        {
            using HttpClient client = new HttpClient();

            string onlineVersion = await client.GetStringAsync(VersionUrl);
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