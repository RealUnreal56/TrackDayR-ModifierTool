using System.Diagnostics;
using System.Windows;

namespace TrackDayRModifier;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Config.Initialize();

        if (Config.getPath() != "")
        {
            MenuWindow menuWindow = new MenuWindow();
            menuWindow.Show();
        }
        else
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }  

        var updateCheck = await VersionControl.CheckForUpdates();

        if (updateCheck.UpdateAvailable)
        {
            MessageBoxResult result = MessageBox.Show(
                "A new version of Modifier Tool is available.\n\n" +
                "Would you like to download the latest version?",
                "Update Available",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information
            );

            if (result == MessageBoxResult.Yes)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://tdrmods.com/posts/modifier-tool",
                    UseShellExecute = true
                });
            }
        }
        
    }
}