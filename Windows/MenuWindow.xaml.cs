using System.Windows;
using System.IO;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;

namespace TrackDayRModifier;

public partial class MenuWindow : Window
{
    public MenuWindow()
    {
        InitializeComponent();

        if (Config.IsFullscreen())
        {
            this.WindowState = WindowState.Maximized;
        }
        else
        {
            this.WindowState = WindowState.Normal;
        }

        // Startseite
        BikesButton.IsChecked = true;
        SettingsButton.IsChecked = false;
        ShowBikes();
    }


    // =========================================================
    // NAVIGATION
    // =========================================================

    private void BikesButton_Click(object sender, RoutedEventArgs e)
    {
        BikesButton.IsChecked = true;
        SettingsButton.IsChecked = false;

        ShowBikes();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        SettingsButton.IsChecked = true;
        BikesButton.IsChecked = false;

        ShowSettings();
    }


    // =========================================================
    // BIKES
    // =========================================================

    private void ShowBikes()
    {
        ContentPanel.Children.Clear();

        PageTitle.Text = "BIKES";
        SearchBarContainer.Visibility = Visibility.Visible;

        DirectoryInfo[] bikeFiles = Files.loadBikes();

        string search = SearchBar.Text.Trim();

        foreach (DirectoryInfo bike in bikeFiles)
        {
            string bikeName = bike.Name;
            string fileName = bike.Name;

            // Suche
            if (!string.IsNullOrWhiteSpace(search))
            {
                bool matchesBikeName =
                    bikeName.Contains(search, StringComparison.OrdinalIgnoreCase);

                bool matchesFileName =
                    fileName.Contains(search, StringComparison.OrdinalIgnoreCase);

                if (!matchesBikeName && !matchesFileName)
                {
                    continue;
                }
            }

            ContentPanel.Children.Add(CreateBikeFrame(bike));
        }
    }


    private Border CreateBikeFrame(DirectoryInfo bike)
    {
        Border frame = new Border
        {
            Height = 100,
            Margin = new Thickness(0, 0, 0, 12),

            Background =
                new SolidColorBrush(Color.FromRgb(28, 28, 30)),

            BorderBrush =
                new SolidColorBrush(Color.FromRgb(45, 45, 47)),

            BorderThickness = new Thickness(1),

            CornerRadius = new CornerRadius(10)
        };


        Grid grid = new Grid
        {
            Margin = new Thickness(20, 12, 15, 12)
        };


        grid.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            }
        );

        grid.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = GridLength.Auto
            }
        );


        // ================= BIKE NAME =================

        StackPanel info = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center
        };


        TextBlock title = new TextBlock
        {
            Text = Path.GetFileNameWithoutExtension(bike.Name),

            Foreground = Brushes.White,

            FontSize = 18,

            FontWeight = FontWeights.SemiBold
        };


        TextBlock fileName = new TextBlock
        {
            Text = bike.Name,

            Foreground =
                new SolidColorBrush(
                    Color.FromRgb(119, 119, 122)
                ),

            FontSize = 11,

            Margin = new Thickness(0, 5, 0, 0)
        };


        info.Children.Add(title);
        info.Children.Add(fileName);


        // ================= MODIFY BUTTON =================

        Button modifyButton = new Button
        {
            Content = "MODIFY",

            Width = 100,

            Height = 36,

            Background =
                new SolidColorBrush(
                    Color.FromRgb(255, 210, 28)
                ),

            Foreground =
                new SolidColorBrush(
                    Color.FromRgb(11, 11, 12)
                ),

            BorderThickness = new Thickness(0),

            FontSize = 12,

            FontWeight = FontWeights.Bold,

            Cursor =
                System.Windows.Input.Cursors.Hand,

            VerticalAlignment =
                VerticalAlignment.Center
        };


        modifyButton.Click += (sender, e) =>
        {
            ModifyBike(bike);
        };


        // ================= ADD =================

        Grid.SetColumn(info, 0);

        Grid.SetColumn(modifyButton, 1);


        grid.Children.Add(info);

        grid.Children.Add(modifyButton);


        frame.Child = grid;


        return frame;
    }


    private void ModifyBike(DirectoryInfo bike)
    {
        ModifyBikeWindow modifyWindow =
            new ModifyBikeWindow(bike);

        modifyWindow.Show();

        this.Close();
    }


    // =========================================================
    // SEARCH
    // =========================================================

    private void SearchBar_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        // Nur auf der Bikes-Seite suchen
        if (BikesButton.IsChecked == true)
        {
            ShowBikes();
        }
    }


    // =========================================================
    // SETTINGS
    // =========================================================

    private void ShowSettings()
    {
        PageTitle.Text = "SETTINGS";
        SearchBarContainer.Visibility = Visibility.Collapsed;
        ContentPanel.Children.Clear();


        // ================= FULLSCREEN =================

        Border fullscreenFrame = new Border
        {
            Height = 80,

            Margin =
                new Thickness(0, 0, 0, 12),

            Background =
                new SolidColorBrush(
                    Color.FromRgb(28, 28, 30)
                ),

            BorderBrush =
                new SolidColorBrush(
                    Color.FromRgb(45, 45, 47)
                ),

            BorderThickness =
                new Thickness(1),

            CornerRadius =
                new CornerRadius(10)
        };


        Grid fullscreenGrid = new Grid
        {
            Margin =
                new Thickness(20, 10, 15, 10)
        };


        TextBlock fullscreenText = new TextBlock
        {
            Text = "ALWAYS FULLSCREEN",

            Foreground = Brushes.White,

            FontSize = 16,

            FontWeight =
                FontWeights.SemiBold,

            VerticalAlignment =
                VerticalAlignment.Center
        };


        CheckBox fullscreenCheckBox = new CheckBox
        {
            Width = 20,

            Height = 20,

            HorizontalAlignment =
                HorizontalAlignment.Right,

            VerticalAlignment =
                VerticalAlignment.Center,

            IsChecked =
                Config.IsFullscreen()
        };


        fullscreenCheckBox.Checked += (sender, e) =>
        {
            Config.SetFullscreen(true);

            this.WindowState =
                WindowState.Maximized;
        };


        fullscreenCheckBox.Unchecked += (sender, e) =>
        {
            Config.SetFullscreen(false);

            this.WindowState =
                WindowState.Normal;
        };


        fullscreenGrid.Children.Add(
            fullscreenText
        );

        fullscreenGrid.Children.Add(
            fullscreenCheckBox
        );


        fullscreenFrame.Child =
            fullscreenGrid;


        ContentPanel.Children.Add(
            fullscreenFrame
        );


        // ================= RESET MOD PATH =================

        Border resetPathFrame = new Border
        {
            Height = 80,

            Margin =
                new Thickness(0, 0, 0, 12),

            Background =
                new SolidColorBrush(
                    Color.FromRgb(28, 28, 30)
                ),

            BorderBrush =
                new SolidColorBrush(
                    Color.FromRgb(45, 45, 47)
                ),

            BorderThickness =
                new Thickness(1),

            CornerRadius =
                new CornerRadius(10)
        };


        Grid resetPathGrid = new Grid
        {
            Margin =
                new Thickness(20, 10, 15, 10)
        };


        TextBlock resetPathText = new TextBlock
        {
            Text = "RESET MOD FOLDER PATH",

            Foreground = Brushes.White,

            FontSize = 16,

            FontWeight =
                FontWeights.SemiBold,

            VerticalAlignment =
                VerticalAlignment.Center
        };


        Button resetPathButton = new Button
        {
            Content = "RESET",

            Width = 100,

            Height = 36,

            HorizontalAlignment =
                HorizontalAlignment.Right,

            VerticalAlignment =
                VerticalAlignment.Center,

            Background =
                new SolidColorBrush(
                    Color.FromRgb(255, 210, 28)
                ),

            Foreground =
                new SolidColorBrush(
                    Color.FromRgb(11, 11, 12)
                ),

            BorderThickness =
                new Thickness(0),

            FontSize = 12,

            FontWeight =
                FontWeights.Bold,

            Cursor =
                System.Windows.Input.Cursors.Hand
        };


        resetPathButton.Click += (sender, e) =>
        {
            Config.savePath("");

            MessageBox.Show(
                "Mod folder path has been cleared.",
                "Info",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );


            MainWindow main =
                new MainWindow();

            main.Show();

            this.Close();
        };


        resetPathGrid.Children.Add(
            resetPathText
        );

        resetPathGrid.Children.Add(
            resetPathButton
        );


        resetPathFrame.Child =
            resetPathGrid;


        ContentPanel.Children.Add(
            resetPathFrame
        );


        // ================= CHECK FOR UPDATES =================

        Border updateFrame = new Border
        {
            Height = 80,

            Margin =
                new Thickness(0, 0, 0, 12),

            Background =
                new SolidColorBrush(
                    Color.FromRgb(28, 28, 30)
                ),

            BorderBrush =
                new SolidColorBrush(
                    Color.FromRgb(45, 45, 47)
                ),

            BorderThickness =
                new Thickness(1),

            CornerRadius =
                new CornerRadius(10)
        };


        Grid updateGrid = new Grid
        {
            Margin =
                new Thickness(20, 10, 15, 10)
        };


        TextBlock updateText = new TextBlock
        {
            Text = "CURRENT VERSION: " + VersionControl.GetCurrentVersion(),

            Foreground = Brushes.White,

            FontSize = 16,

            FontWeight =
                FontWeights.SemiBold,

            VerticalAlignment =
                VerticalAlignment.Center
        };


        Button updateButton = new Button
        {
            Content = "CHECK FOR UPDATES",

            Width = 160,

            Height = 36,

            HorizontalAlignment =
                HorizontalAlignment.Right,

            VerticalAlignment =
                VerticalAlignment.Center,

            Background =
                new SolidColorBrush(
                    Color.FromRgb(255, 210, 28)
                ),

            Foreground =
                new SolidColorBrush(
                    Color.FromRgb(11, 11, 12)
                ),

            BorderThickness =
                new Thickness(0),

            FontSize = 12,

            FontWeight =
                FontWeights.Bold,

            Cursor =
                System.Windows.Input.Cursors.Hand
        };


        updateButton.Click += async (sender, e) =>
        {   
            updateButton.Content = "CHECKING...";
            var updateCheck = await VersionControl.CheckForUpdates();

            bool updateAvailable = updateCheck.UpdateAvailable;
            string onlineVersion = updateCheck.OnlineVersion;

            updateButton.Content = "CHECK FOR UPDATES";
            if (updateAvailable)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"A new version of Modifier Tool is available.\n\n" +
                    $"Installed version: {VersionControl.GetCurrentVersion()}\n" +
                    $"Latest version: {onlineVersion}\n\n" +
                    $"Would you like to download the latest version?",
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
            else
            {
                MessageBox.Show(
                    "Modifier Tool is up to date!",
                    "No Update Available",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        };


        updateGrid.Children.Add(
            updateText
        );

        updateGrid.Children.Add(
            updateButton
        );


        updateFrame.Child =
            updateGrid;


        ContentPanel.Children.Add(
            updateFrame
        );
    }
}