using System.Diagnostics;
using System.Windows;
using System.IO;

namespace ModifierTool;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Config.Initialize();

        var updateCheck = await VersionControl.CheckForUpdates();

        if (Config.getModPath() != "")
        {
            MenuWindow menuWindow = new MenuWindow();

            MainWindow = menuWindow;
            menuWindow.Show();
        }
        else
        {
            bool normalPathExists = CheckDefaultModPath();

            if(normalPathExists) return;

            MainWindow mainWindow = new MainWindow();

            MainWindow = mainWindow;
            mainWindow.Show();
        }

        
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

    //Tries to find the mod folder automatically
    private bool CheckDefaultModPath()
    {
        string defaultPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "TrackdayR"
        );

        if (Directory.Exists(defaultPath))
        {
            Config.saveModPath(defaultPath);
            
            MenuWindow menuWindow = new MenuWindow();
            menuWindow.Show();

            MessageBox.Show(
                "Your TrackDayR mod folder has been recognized automatically.\n\n" +
                "If this is the wrong folder go to SETTINGS > RESET MOD FOLDER PATH.",
                "Mod folder recognized",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            return true;
        }
        return false;
    }
}