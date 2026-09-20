using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;


namespace ModifierTool;

public static class ModificationManager
{
    public static void SaveModification(FileInfo tdr)
    {
        Window window = new Window
        {
            Title = "Save Modification",
            Width = 400,
            Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
            Background = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(22, 22, 24)
            )
        };

        Grid grid = new Grid
        {
            Margin = new Thickness(20)
        };

        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());

        TextBlock label = new TextBlock
        {
            Text = "Enter a name for the modification:",
            Foreground = System.Windows.Media.Brushes.White,
            FontSize = 14
        };

        Grid.SetRow(label, 0);
        grid.Children.Add(label);

        TextBox nameBox = new TextBox
        {
            Text = Path.GetFileNameWithoutExtension(tdr.Name),
            Height = 35,
            Margin = new Thickness(0, 5, 0, 5)
        };

        Grid.SetRow(nameBox, 1);
        grid.Children.Add(nameBox);

        Button saveButton = new Button
        {
            Content = "SAVE",
            Width = 90,
            Height = 35,
            HorizontalAlignment = HorizontalAlignment.Left,
            Background = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(255, 210, 28)
            ),
            Foreground = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(11, 11, 12)
            )
        };

        Grid.SetRow(saveButton, 2);
        grid.Children.Add(saveButton);

        saveButton.Click += (sender, e) =>
        {
            if (string.IsNullOrWhiteSpace(nameBox.Text))
                return;

            window.DialogResult = true;
            window.Close();
        };

        window.Content = grid;

        if (window.ShowDialog() != true)
            return;

        string name = nameBox.Text.Trim();

        OpenFolderDialog dialog = new OpenFolderDialog
        {
            Title = "Select Folder"
        };

        if (dialog.ShowDialog() != true)
            return;

        string filePath = Path.Combine(
            dialog.FolderName,
            name + ".tdr"
        );

        File.Copy(
            tdr.FullName,
            filePath,
            true
        );

        MessageBox.Show(
            "Modification saved successfully!",
            "Saved",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
    }

    public static FileInfo LoadModification()
    {
        MessageBoxResult result = MessageBox.Show(
            "Loading a modification will overwrite the current modifications of your bike. Do you want to continue?",
            "Warning",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );

        if (result == MessageBoxResult.Yes)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select Modification",
                Filter = "TrackDayR Modification (*.tdr)|*.tdr",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                string filePath = dialog.FileName;

                FileInfo tdr = new FileInfo(filePath);
                return tdr;
            }
        }

        return null;
    }
}