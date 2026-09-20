using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace ModifierTool;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        OpenFolderDialog dialog = new OpenFolderDialog
        {
            Title = "TrackDayR-Ordner auswählen"
        };

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            FilePathTextBox.Text = dialog.FolderName;
        }
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        string path = FilePathTextBox.Text;

        if (string.IsNullOrWhiteSpace(path))
        {
            MessageBox.Show("Please select a valid path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Config.savePath(path);

        // Open the next window
        MenuWindow menuWindow = new MenuWindow();
        menuWindow.Show();

        this.Close();
    }
}