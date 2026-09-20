using System.Windows;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.IO;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace ModifierTool;

public partial class ModifyBikeWindow : Window
{
    private DirectoryInfo bike;
    private JsonObject tdr;
    private FileInfo tdrFile;

    public ModifyBikeWindow(DirectoryInfo _bike)
    {
        InitializeComponent();

        bike = _bike;

        tdrFile = TDRreader.GetTDR(bike);

        tdr = JsonNode.Parse(
            File.ReadAllText(tdrFile.FullName)
        )!.AsObject();

        if (Config.IsFullscreen())
        {
            this.WindowState = WindowState.Maximized;
        }
        else
        {
            this.WindowState = WindowState.Normal;
        }

        BikeNameText.Text = bike.Name;

        ShowEngine();
    }


    // =========================
    // TOP BAR
    // =========================

    private void EngineButton_Click(object sender, RoutedEventArgs e)
    {
        ShowEngine();
    }

    private void GearboxButton_Click(object sender, RoutedEventArgs e)
    {
        ShowGearbox();
    }

    private void OtherButton_Click(object sender, RoutedEventArgs e)
    {
        ShowOther();
    }

    private void ModificationButton_Click(object sender, RoutedEventArgs e)
    {
        ShowLoadSaveModification();
    }
    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        MenuWindow menuWindow = new MenuWindow();
        menuWindow.Show();

        this.Close();
    }



    // =========================
    // ENGINE
    // =========================

    private void ShowEngine()
    {
        ContentPanel.Children.Clear();

        JsonObject drivetrain =
            tdr["drivetrain"]!.AsObject();

        int enginePoints =
            drivetrain["EnginePoint"]!.GetValue<int>();

        JsonArray enginePower =
            drivetrain["EnginePower"]!.AsArray();


        // =========================
        // ENGINE POWER GRAPH
        // =========================

        Border graphFrame = new Border
        {
            Height = 300,
            Background = new SolidColorBrush(
                Color.FromRgb(28, 28, 30)
            ),
            BorderBrush = new SolidColorBrush(
                Color.FromRgb(45, 45, 47)
            ),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(20),
            Margin = new Thickness(0, 0, 0, 15)
        };

        Grid graphGrid = new Grid();

        Canvas graphCanvas = new Canvas
        {
            Background = new SolidColorBrush(
                Color.FromRgb(20, 20, 21)
            )
        };

        graphGrid.Children.Add(graphCanvas);

        graphFrame.Child = graphGrid;

        ContentPanel.Children.Add(graphFrame);


        // Graph zeichnen sobald Canvas geladen ist
        graphCanvas.Loaded += (sender, e) =>
        {
            DrawEngineGraph(graphCanvas);
        };

        // =========================
        // IDLE / LIMITER SETTINGS
        // =========================

        Border rpmSettingsFrame = new Border
        {
            Background = new SolidColorBrush(
                Color.FromRgb(28, 28, 30)
            ),
            BorderBrush = new SolidColorBrush(
                Color.FromRgb(45, 45, 47)
            ),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(15),
            Margin = new Thickness(0, 0, 0, 15)
        };


        Grid rpmSettingsGrid = new Grid();

        rpmSettingsGrid.RowDefinitions.Add(
            new RowDefinition
            {
                Height = GridLength.Auto
            }
        );

        rpmSettingsGrid.RowDefinitions.Add(
            new RowDefinition
            {
                Height = GridLength.Auto
            }
        );

        rpmSettingsGrid.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            }
        );

        rpmSettingsGrid.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            }
        );

        rpmSettingsGrid.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = GridLength.Auto
            }
        );


        // =========================
        // TITLE
        // =========================

        TextBlock rpmSettingsTitle = new TextBlock
        {
            Text = "RPM SETTINGS",
            Foreground = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            ),
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 12)
        };

        Grid.SetRow(rpmSettingsTitle, 0);
        Grid.SetColumn(rpmSettingsTitle, 0);
        Grid.SetColumnSpan(rpmSettingsTitle, 3);

        rpmSettingsGrid.Children.Add(rpmSettingsTitle);


        // =========================
        // CURRENT VALUES
        // =========================

        double idle =
            drivetrain["idle"]!.GetValue<double>();

        double limiter =
            drivetrain["limiter"]!.GetValue<double>();


        // =========================
        // IDLE
        // =========================

        StackPanel idlePanel = new StackPanel
        {
            Margin = new Thickness(0, 0, 10, 0)
        };

        TextBlock idleLabel = new TextBlock
        {
            Text = "IDLE",
            Foreground = Brushes.Gray,
            FontSize = 11,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 5)
        };

        TextBox idleInput = new TextBox
        {
            Text = idle.ToString(
                CultureInfo.InvariantCulture
            ),
            Height = 36,
            Padding = new Thickness(10, 8, 10, 8),
            Background = new SolidColorBrush(
                Color.FromRgb(20, 20, 21)
            ),
            Foreground = Brushes.White,
            BorderBrush = new SolidColorBrush(
                Color.FromRgb(55, 55, 57)
            ),
            BorderThickness = new Thickness(1),
            FontSize = 14
        };

        idlePanel.Children.Add(idleLabel);
        idlePanel.Children.Add(idleInput);


        // =========================
        // LIMITER
        // =========================

        StackPanel limiterPanel = new StackPanel
        {
            Margin = new Thickness(10, 0, 10, 0)
        };

        TextBlock limiterLabel = new TextBlock
        {
            Text = "RPM LIMITER",
            Foreground = Brushes.Gray,
            FontSize = 11,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 5)
        };

        TextBox limiterInput = new TextBox
        {
            Text = limiter.ToString(
                CultureInfo.InvariantCulture
            ),
            Height = 36,
            Padding = new Thickness(10, 8, 10, 8),
            Background = new SolidColorBrush(
                Color.FromRgb(20, 20, 21)
            ),
            Foreground = Brushes.White,
            BorderBrush = new SolidColorBrush(
                Color.FromRgb(55, 55, 57)
            ),
            BorderThickness = new Thickness(1),
            FontSize = 14
        };

        limiterPanel.Children.Add(limiterLabel);
        limiterPanel.Children.Add(limiterInput);


        // =========================
        // SAVE BUTTON
        // =========================

        Button saveRpmButton = new Button
        {
            Content = "SAVE",
            Width = 90,
            Height = 36,
            Margin = new Thickness(10, 20, 0, 0),
            Background = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            ),
            Foreground = new SolidColorBrush(
                Color.FromRgb(11, 11, 12)
            ),
            BorderThickness = new Thickness(0),
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Cursor = System.Windows.Input.Cursors.Hand
        };


        // =========================
        // SAVE RPM SETTINGS
        // =========================

        saveRpmButton.Click += async (sender, e) =>
        {
            if (!double.TryParse(
                    idleInput.Text,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double newIdle))
            {
                MessageBox.Show(
                    "Invalid idle RPM value.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                return;
            }


            if (!double.TryParse(
                    limiterInput.Text,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double newLimiter))
            {
                MessageBox.Show(
                    "Invalid limiter RPM value.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                return;
            }


            // JSON aktualisieren

            drivetrain["idle"] = newIdle;
            drivetrain["limiter"] = newLimiter;


            // TDR speichern

            SaveTDR();


            // Graph aktualisieren

            DrawEngineGraph(graphCanvas);


            // =========================
            // SAVE EFFECT
            // =========================

            saveRpmButton.Content = "✓ SAVED";

            saveRpmButton.Background = new SolidColorBrush(
                Color.FromRgb(60, 130, 70)
            );

            saveRpmButton.Foreground = Brushes.White;


            await Task.Delay(1000);


            saveRpmButton.Content = "SAVE";

            saveRpmButton.Background = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            );

            saveRpmButton.Foreground = new SolidColorBrush(
                Color.FromRgb(11, 11, 12)
            );
        };


        // =========================
        // GRID POSITION
        // =========================

        Grid.SetRow(idlePanel, 1);
        Grid.SetColumn(idlePanel, 0);

        Grid.SetRow(limiterPanel, 1);
        Grid.SetColumn(limiterPanel, 1);

        Grid.SetRow(saveRpmButton, 1);
        Grid.SetColumn(saveRpmButton, 2);


        rpmSettingsGrid.Children.Add(idlePanel);
        rpmSettingsGrid.Children.Add(limiterPanel);
        rpmSettingsGrid.Children.Add(saveRpmButton);


        rpmSettingsFrame.Child = rpmSettingsGrid;

        ContentPanel.Children.Add(rpmSettingsFrame);
        // =========================
        // ENGINE POINTS
        // =========================

        int index = 0;

        foreach (JsonNode? pointNode in enginePower)
        {
            if (pointNode == null)
                continue;

            JsonObject point = pointNode.AsObject();

            double rpm =
                point["rpm"]!.GetValue<double>();

            double nm =
                point["Nm"]!.GetValue<double>();


            Border frame = new Border
            {
                Background = new SolidColorBrush(
                    Color.FromRgb(28, 28, 30)
                ),
                BorderBrush = new SolidColorBrush(
                    Color.FromRgb(45, 45, 47)
                ),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 10)
            };


            Grid grid = new Grid();


            // Zeilen

            grid.RowDefinitions.Add(
                new RowDefinition
                {
                    Height = GridLength.Auto
                }
            );

            grid.RowDefinitions.Add(
                new RowDefinition
                {
                    Height = GridLength.Auto
                }
            );


            // Spalten

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = new GridLength(
                        1,
                        GridUnitType.Star
                    )
                }
            );

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = new GridLength(
                        1,
                        GridUnitType.Star
                    )
                }
            );

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = GridLength.Auto
                }
            );

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = GridLength.Auto
                }
            );


            int pointIndex = index;


            // =========================
            // POINT TEXT
            // =========================

            TextBlock pointText = new TextBlock
            {
                Text = $"POINT {pointIndex + 1}",
                Foreground = new SolidColorBrush(
                    Color.FromRgb(255, 210, 28)
                ),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 12)
            };

            Grid.SetRow(pointText, 0);
            Grid.SetColumn(pointText, 0);
            Grid.SetColumnSpan(pointText, 3);

            grid.Children.Add(pointText);


            // =========================
            // RPM
            // =========================

            StackPanel rpmPanel = new StackPanel
            {
                Margin = new Thickness(0, 0, 10, 0)
            };


            TextBlock rpmLabel = new TextBlock
            {
                Text = "RPM",
                Foreground = Brushes.Gray,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };


            TextBox rpmInput = new TextBox
            {
                Text = rpm.ToString(
                    CultureInfo.InvariantCulture
                ),
                Height = 36,
                Padding = new Thickness(10, 8, 10, 8),
                Background = new SolidColorBrush(
                    Color.FromRgb(20, 20, 21)
                ),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(
                    Color.FromRgb(55, 55, 57)
                ),
                BorderThickness = new Thickness(1),
                FontSize = 14
            };


            rpmPanel.Children.Add(rpmLabel);
            rpmPanel.Children.Add(rpmInput);


            // =========================
            // NM
            // =========================

            StackPanel nmPanel = new StackPanel
            {
                Margin = new Thickness(10, 0, 10, 0)
            };


            TextBlock nmLabel = new TextBlock
            {
                Text = "TORQUE (Nm)",
                Foreground = Brushes.Gray,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };


            TextBox nmInput = new TextBox
            {
                Text = nm.ToString(
                    CultureInfo.InvariantCulture
                ),
                Height = 36,
                Padding = new Thickness(10, 8, 10, 8),
                Background = new SolidColorBrush(
                    Color.FromRgb(20, 20, 21)
                ),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(
                    Color.FromRgb(55, 55, 57)
                ),
                BorderThickness = new Thickness(1),
                FontSize = 14
            };


            nmPanel.Children.Add(nmLabel);
            nmPanel.Children.Add(nmInput);


            // =========================
            // SAVE BUTTON
            // =========================

            Button saveButton = new Button
            {
                Content = "SAVE",
                Width = 90,
                Height = 36,
                Margin = new Thickness(10, 20, 5, 0),
                Background = new SolidColorBrush(
                    Color.FromRgb(255, 210, 28)
                ),
                Foreground = new SolidColorBrush(
                    Color.FromRgb(11, 11, 12)
                ),
                BorderThickness = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand
            };


            // =========================
            // DELETE BUTTON
            // =========================

            Button deleteButton = new Button
            {
                Content = "DELETE",
                Width = 90,
                Height = 36,
                Margin = new Thickness(5, 20, 0, 0),
                Background = new SolidColorBrush(
                    Color.FromRgb(75, 35, 35)
                ),
                Foreground = new SolidColorBrush(
                    Color.FromRgb(255, 130, 130)
                ),
                BorderBrush = new SolidColorBrush(
                    Color.FromRgb(110, 50, 50)
                ),
                BorderThickness = new Thickness(1),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand
            };


            // =========================
            // SAVE CLICK
            // =========================

            saveButton.Click += async (sender, e) =>
            {
                if (!double.TryParse(
                        rpmInput.Text,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out double newRpm))
                {
                    MessageBox.Show(
                        "Invalid RPM value.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );

                    return;
                }


                if (!double.TryParse(
                        nmInput.Text,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out double newNm))
                {
                    MessageBox.Show(
                        "Invalid Nm value.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );

                    return;
                }


                // JSON aktualisieren

                point["rpm"] = newRpm;
                point["Nm"] = newNm;


                // TDR speichern

                SaveTDR();


                // Graph aktualisieren

                DrawEngineGraph(graphCanvas);


                // =========================
                // SAVE EFFECT
                // =========================

                saveButton.Content = "✓ SAVED";

                saveButton.Background = new SolidColorBrush(
                    Color.FromRgb(60, 130, 70)
                );

                saveButton.Foreground = Brushes.White;


                await Task.Delay(1000);


                saveButton.Content = "SAVE";

                saveButton.Background = new SolidColorBrush(
                    Color.FromRgb(255, 210, 28)
                );

                saveButton.Foreground = new SolidColorBrush(
                    Color.FromRgb(11, 11, 12)
                );


                Console.WriteLine(
                    $"Point {pointIndex + 1}: RPM={newRpm}, Nm={newNm}"
                );
            };

            // =========================
            // Delete CLICK
            // =========================
            deleteButton.Click += (sender, e) =>
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Delete POINT {pointIndex + 1}?",
                    "Delete Engine Point",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result != MessageBoxResult.Yes)
                    return;

                try
                {
                    // Punkt aus EnginePower entfernen
                    enginePower.Remove(point);

                    // EnginePoint um 1 reduzieren
                    int currentEnginePoints =
                        drivetrain["EnginePoint"]!.GetValue<int>();

                    if (currentEnginePoints > 0)
                    {
                        drivetrain["EnginePoint"] =
                            currentEnginePoints - 1;
                    }

                    // TDR speichern
                    SaveTDR();

                    // Seite neu aufbauen
                    ShowEngine();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.ToString(),
                        "Error deleting engine point",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            };

            // =========================
            // GRID POSITION
            // =========================

            Grid.SetRow(rpmPanel, 1);
            Grid.SetColumn(rpmPanel, 0);

            Grid.SetRow(nmPanel, 1);
            Grid.SetColumn(nmPanel, 1);

            Grid.SetRow(saveButton, 1);
            Grid.SetColumn(saveButton, 2);

            Grid.SetRow(deleteButton, 1);
            Grid.SetColumn(deleteButton, 3);


            grid.Children.Add(rpmPanel);
            grid.Children.Add(nmPanel);
            grid.Children.Add(saveButton);
            grid.Children.Add(deleteButton);


            frame.Child = grid;

            ContentPanel.Children.Add(frame);


            index++;
        }
    
        // =========================
        // ADD ENGINE POINT BUTTON
        // =========================

        Button addEnginePointButton = new Button
        {
            Content = "+",
            Height = 100,
            Margin = new Thickness(0, 0, 0, 10),
            Background = new SolidColorBrush(
                Color.FromRgb(28, 28, 30)
            ),
            Foreground = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            ),
            BorderBrush = new SolidColorBrush(
                Color.FromRgb(45, 45, 47)
            ),
            BorderThickness = new Thickness(1),
            FontSize = 32,
            FontWeight = FontWeights.Bold,
            Cursor = System.Windows.Input.Cursors.Hand
        };

        addEnginePointButton.Click += (sender, e) =>
        {
            try
            {
                // Neuen Punkt anlegen
                JsonObject newPoint = new JsonObject
                {
                    ["rpm"] = 0.0,
                    ["Nm"] = 0.0
                };

                // Punkt hinzufügen
                enginePower.Add(newPoint);

                // EnginePoint erhöhen
                int currentEnginePoints =
                    drivetrain["EnginePoint"]!.GetValue<int>();

                drivetrain["EnginePoint"] = currentEnginePoints + 1;

                // Speichern
                SaveTDR();

                // Seite neu laden
                ShowEngine();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Fehler beim Neuladen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        };

        ContentPanel.Children.Add(addEnginePointButton);
    }


    // =========================
    // SAVE TDR
    // =========================

    private void SaveTDR()
    {
        try
        {
            JsonSerializerOptions options =
                new JsonSerializerOptions
                {
                    WriteIndented = true
                };

            string json =
                tdr.ToJsonString(options);

            File.WriteAllText(
                tdrFile.FullName,
                json
            );

            Console.WriteLine(
                $"Saved TDR: {tdrFile.FullName}"
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not save TDR file:\n\n{ex.Message}",
                "Save Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }


    // =========================
    // ENGINE GRAPH
    // =========================

    private void DrawEngineGraph(Canvas canvas)
    {
        canvas.Children.Clear();

        double width = canvas.ActualWidth;
        double height = canvas.ActualHeight;

        if (width <= 0 || height <= 0)
            return;


        double left = 55;
        double right = 20;
        double top = 25;
        double bottom = 40;


        double graphWidth =
            width - left - right;

        double graphHeight =
            height - top - bottom;


        JsonArray enginePower =
            tdr["drivetrain"]!
            .AsObject()["EnginePower"]!
            .AsArray();

        double limiter =
            tdr["drivetrain"]!
            .AsObject()["limiter"]!.GetValue<double>();

        double idle =
            tdr["drivetrain"]!
            .AsObject()["idle"]!.GetValue<double>();


        List<(double rpm, double nm)> points = new();


        foreach (JsonNode? pointNode in enginePower)
        {
            if (pointNode == null)
                continue;

            JsonObject point =
                pointNode.AsObject();

            double rpm =
                point["rpm"]!.GetValue<double>();

            double nm =
                point["Nm"]!.GetValue<double>();

            points.Add((rpm, nm));
        }


        if (points.Count == 0)
            return;


        // =========================
        // RANGE
        // =========================

        double minRpm = 0;

        double maxRpm =
            points.Max(p => p.rpm);


        double minNm = 0;

        double maxNm =
            Math.Ceiling(
                points.Max(p => p.nm) / 5.0
            ) * 5.0;


        if (maxRpm == minRpm)
            maxRpm++;


        if (maxNm <= minNm)
            maxNm = minNm + 5;


        // =========================
        // GRID
        // =========================

        int gridLines = 5;


        for (int i = 0; i <= gridLines; i++)
        {
            double y =
                top +
                graphHeight -
                (i / (double)gridLines) *
                graphHeight;


            Line gridLine = new Line
            {
                X1 = left,
                Y1 = y,
                X2 = left + graphWidth,
                Y2 = y,

                Stroke = new SolidColorBrush(
                    Color.FromRgb(45, 45, 47)
                ),

                StrokeThickness = 1
            };


            canvas.Children.Add(gridLine);


            double value =
                minNm +
                (maxNm - minNm) *
                i /
                gridLines;


            TextBlock label = new TextBlock
            {
                Text = value.ToString("0"),
                Foreground = Brushes.Gray,
                FontSize = 10,
                Width = 45,
                TextAlignment = TextAlignment.Right
            };


            Canvas.SetLeft(label, 5);
            Canvas.SetTop(label, y - 7);


            canvas.Children.Add(label);
        }


        // =========================
        // X AXIS
        // =========================

        Line xAxis = new Line
        {
            X1 = left,
            Y1 = top + graphHeight,

            X2 = left + graphWidth,
            Y2 = top + graphHeight,

            Stroke = new SolidColorBrush(
                Color.FromRgb(100, 100, 100)
            ),

            StrokeThickness = 1
        };


        canvas.Children.Add(xAxis);


        // =========================
        // Y AXIS
        // =========================

        Line yAxis = new Line
        {
            X1 = left,
            Y1 = top,

            X2 = left,
            Y2 = top + graphHeight,

            Stroke = new SolidColorBrush(
                Color.FromRgb(100, 100, 100)
            ),

            StrokeThickness = 1
        };


        canvas.Children.Add(yAxis);


        // =========================
        // X LABELS
        // =========================

        int xLabels =
            Math.Min(points.Count, 6);


        for (int i = 0; i < xLabels; i++)
        {
            double rpm;


            if (xLabels == 1)
            {
                rpm = minRpm;
            }
            else
            {
                rpm =
                    minRpm +
                    (maxRpm - minRpm) *
                    i /
                    (xLabels - 1);
            }


            double x =
                left +
                ((rpm - minRpm) /
                (maxRpm - minRpm)) *
                graphWidth;


            TextBlock label = new TextBlock
            {
                Text = rpm.ToString("0"),
                Foreground = Brushes.Gray,
                FontSize = 10,
                Width = 60,
                TextAlignment = TextAlignment.Center
            };


            Canvas.SetLeft(
                label,
                x - 30
            );

            Canvas.SetTop(
                label,
                top + graphHeight + 8
            );


            canvas.Children.Add(label);
        }


        // =========================
        // GRAPH LINE
        // =========================

        Polyline line = new Polyline
        {
            Stroke = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            ),

            StrokeThickness = 3
        };


        // Start bei 0 RPM / 0 Nm
        line.Points.Add(
            new Point(
                left,
                top + graphHeight
            )
        );


        // EnginePower Punkte
        foreach ((double rpm, double nm) point in points)
        {
            double x =
                left +
                ((point.rpm - minRpm) /
                (maxRpm - minRpm)) *
                graphWidth;


            double y =
                top +
                graphHeight -
                ((point.nm - minNm) /
                (maxNm - minNm)) *
                graphHeight;


            line.Points.Add(
                new Point(x, y)
            );
        }


        canvas.Children.Add(line);


        // =========================
        // GRAPH POINTS
        // =========================

        foreach ((double rpm, double nm) point in points)
        {
            double x =
                left +
                ((point.rpm - minRpm) /
                (maxRpm - minRpm)) *
                graphWidth;


            double y =
                top +
                graphHeight -
                ((point.nm - minNm) /
                (maxNm - minNm)) *
                graphHeight;


            Ellipse circle = new Ellipse
            {
                Width = 8,
                Height = 8,

                Fill = new SolidColorBrush(
                    Color.FromRgb(255, 210, 28)
                )
            };


            Canvas.SetLeft(
                circle,
                x - 4
            );

            Canvas.SetTop(
                circle,
                y - 4
            );


            canvas.Children.Add(circle);
        }


        // =========================
        // LIMITER AND IDLE LINE
        // =========================

        double limiterX =
            left +
            ((limiter - minRpm) /
            (maxRpm - minRpm)) *
            graphWidth;


        Line limiterLine = new Line
        {
            X1 = limiterX,
            Y1 = top,

            X2 = limiterX,
            Y2 = top + graphHeight,

            Stroke = new SolidColorBrush(
                Color.FromRgb(255, 0, 0)
            ),

            StrokeThickness = 2,
            StrokeDashArray = new DoubleCollection { 4, 4 }
        };


        canvas.Children.Add(limiterLine);


        double idleX =
            left +
            ((idle - minRpm) /
            (maxRpm - minRpm)) *
            graphWidth;


        Line idleLine = new Line
        {
            X1 = idleX,
            Y1 = top,

            X2 = idleX,
            Y2 = top + graphHeight,

            Stroke = new SolidColorBrush(
                Color.FromRgb(0, 0, 255)
            ),

            StrokeThickness = 2,
            StrokeDashArray = new DoubleCollection { 4, 4 }
        };


        canvas.Children.Add(idleLine);


        // =========================
        // LIMITER LABEL
        // =========================

        TextBlock limiterLabel = new TextBlock
        {
            Text = "RPM LIMITER",
            Foreground = Brushes.Red,
            FontSize = 10,
            FontWeight = FontWeights.Bold
        };


        Canvas.SetLeft(
            limiterLabel,
            limiterX + 5
        );

        Canvas.SetTop(
            limiterLabel,
            top + 5
        );


        canvas.Children.Add(limiterLabel);


        // =========================
        // IDLE LABEL
        // =========================

        TextBlock idleLabel = new TextBlock
        {
            Text = "IDLE",
            Foreground = Brushes.Blue,
            FontSize = 10,
            FontWeight = FontWeights.Bold
        };


        Canvas.SetLeft(
            idleLabel,
            idleX + 5
        );

        Canvas.SetTop(
            idleLabel,
            top + 5
        );


        canvas.Children.Add(idleLabel);


        // =========================
        // Y TITLE
        // =========================

        TextBlock nmTitle = new TextBlock
        {
            Text = "Nm",
            Foreground = Brushes.White,
            FontSize = 11,
            FontWeight = FontWeights.Bold
        };


        Canvas.SetLeft(nmTitle, 5);
        Canvas.SetTop(nmTitle, 5);


        canvas.Children.Add(nmTitle);


        // =========================
        // X TITLE
        // =========================

        TextBlock rpmTitle = new TextBlock
        {
            Text = "RPM",
            Foreground = Brushes.White,
            FontSize = 11,
            FontWeight = FontWeights.Bold
        };


        Canvas.SetLeft(
            rpmTitle,
            width - 50
        );

        Canvas.SetTop(
            rpmTitle,
            height - 22
        );


        canvas.Children.Add(rpmTitle);
    }
    // =========================
    // GEARBOX
    // =========================

    private void ShowGearbox()
    {
        ContentPanel.Children.Clear();

        JsonObject drivetrain = tdr["drivetrain"]!.AsObject();
        JsonArray gears = drivetrain["Gears"]!.AsArray();

        int gearNumber = 1;

        for (int i = 0; i < gears.Count; i++)
        {
            double ratio = gears[i]!.GetValue<double>();

            int arrayIndex = i;

            bool isNeutral = ratio == 0;

            string gearName = isNeutral
                ? "NEUTRAL"
                : $"GEAR {gearNumber}";


            // =========================
            // FRAME
            // =========================

            Border frame = new Border
            {
                Background = new SolidColorBrush(
                    Color.FromRgb(28, 28, 30)
                ),
                BorderBrush = new SolidColorBrush(
                    Color.FromRgb(45, 45, 47)
                ),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(15),
                Margin = new Thickness(0, 0, 0, 10)
            };


            Grid grid = new Grid();


            // =========================
            // ROWS
            // =========================

            grid.RowDefinitions.Add(
                new RowDefinition
                {
                    Height = GridLength.Auto
                }
            );

            grid.RowDefinitions.Add(
                new RowDefinition
                {
                    Height = GridLength.Auto
                }
            );


            // =========================
            // COLUMNS
            // =========================

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = new GridLength(
                        1,
                        GridUnitType.Star
                    )
                }
            );

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = GridLength.Auto
                }
            );

            grid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = GridLength.Auto
                }
            );


            // =========================
            // GEAR TEXT
            // =========================

            TextBlock gearText = new TextBlock
            {
                Text = gearName,
                Foreground = isNeutral
                    ? Brushes.Gray
                    : new SolidColorBrush(
                        Color.FromRgb(255, 210, 28)
                    ),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 12)
            };

            Grid.SetRow(gearText, 0);
            Grid.SetColumn(gearText, 0);
            Grid.SetColumnSpan(gearText, 3);

            grid.Children.Add(gearText);


            // =========================
            // RATIO PANEL
            // =========================

            StackPanel ratioPanel = new StackPanel
            {
                Margin = new Thickness(0, 0, 10, 0)
            };


            TextBlock ratioLabel = new TextBlock
            {
                Text = "RATIO",
                Foreground = Brushes.Gray,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };


            TextBox ratioInput = new TextBox
            {
                Text = ratio.ToString(
                    CultureInfo.InvariantCulture
                ),
                Height = 36,
                Padding = new Thickness(10, 8, 10, 8),
                Background = new SolidColorBrush(
                    Color.FromRgb(20, 20, 21)
                ),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(
                    Color.FromRgb(55, 55, 57)
                ),
                BorderThickness = new Thickness(1),
                FontSize = 14
            };


            ratioPanel.Children.Add(ratioLabel);
            ratioPanel.Children.Add(ratioInput);


            Grid.SetRow(ratioPanel, 1);
            Grid.SetColumn(ratioPanel, 0);

            grid.Children.Add(ratioPanel);


            // =========================
            // SAVE BUTTON
            // =========================

            Button saveButton = new Button
            {
                Content = "SAVE",
                Width = 90,
                Height = 36,
                Margin = new Thickness(10, 20, 5, 0),
                Background = new SolidColorBrush(
                    Color.FromRgb(255, 210, 28)
                ),
                Foreground = new SolidColorBrush(
                    Color.FromRgb(11, 11, 12)
                ),
                BorderThickness = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            Grid.SetRow(saveButton, 1);
            Grid.SetColumn(saveButton, 1);

            grid.Children.Add(saveButton);


            // =========================
            // DELETE BUTTON
            // =========================

            Button deleteButton = new Button
            {
                Content = "DELETE",
                Width = 90,
                Height = 36,
                Margin = new Thickness(5, 20, 0, 0),
                Background = new SolidColorBrush(
                    Color.FromRgb(75, 35, 35)
                ),
                Foreground = new SolidColorBrush(
                    Color.FromRgb(255, 130, 130)
                ),
                BorderBrush = new SolidColorBrush(
                    Color.FromRgb(110, 50, 50)
                ),
                BorderThickness = new Thickness(1),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            Grid.SetRow(deleteButton, 1);
            Grid.SetColumn(deleteButton, 2);

            grid.Children.Add(deleteButton);


            // =========================
            // SAVE CLICK
            // =========================

            saveButton.Click += async (sender, e) =>
            {
                if (!double.TryParse(
                        ratioInput.Text,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out double newRatio))
                {
                    MessageBox.Show(
                        "Invalid ratio value.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );

                    return;
                }


                gears[arrayIndex] = newRatio;

                SaveTDR();


                saveButton.Content = "✓ SAVED";

                saveButton.Background = new SolidColorBrush(
                    Color.FromRgb(60, 130, 70)
                );

                saveButton.Foreground = Brushes.White;


                await Task.Delay(1000);


                saveButton.Content = "SAVE";

                saveButton.Background = new SolidColorBrush(
                    Color.FromRgb(255, 210, 28)
                );

                saveButton.Foreground = new SolidColorBrush(
                    Color.FromRgb(11, 11, 12)
                );

                
                ShowGearbox();
            };

            // =========================
            // DELETE CLICK
            // =========================

            deleteButton.Click += (sender, e) =>
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Delete {gearName}?",
                    "Delete Gear",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result != MessageBoxResult.Yes)
                    return;


                try
                {
                    // Gang / Neutral aus dem Array entfernen

                    gears.RemoveAt(arrayIndex);


                    // numberOfGear reduzieren

                    int currentNumberOfGears =
                        drivetrain["numberOfGear"]!.GetValue<int>();

                    if (currentNumberOfGears > 0)
                    {
                        drivetrain["numberOfGear"] =
                            currentNumberOfGears - 1;
                    }


                    // TDR speichern

                    SaveTDR();


                    // Seite neu aufbauen

                    ShowGearbox();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.ToString(),
                        "Error deleting gear",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            };


            // =========================
            // FRAME
            // =========================

            frame.Child = grid;

            ContentPanel.Children.Add(frame);


            // Nur echte Gänge hochzählen

            if (!isNeutral)
            {
                gearNumber++;
            }
        }

        // =========================
        // ADD GEAR BUTTON
        // =========================

        Button addGearButton = new Button
        {
            Content = "+",
            Height = 100,
            Margin = new Thickness(0, 0, 0, 10),
            Background = new SolidColorBrush(
                Color.FromRgb(28, 28, 30)
            ),
            Foreground = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            ),
            BorderBrush = new SolidColorBrush(
                Color.FromRgb(45, 45, 47)
            ),
            BorderThickness = new Thickness(1),
            FontSize = 32,
            FontWeight = FontWeights.Bold,
            Cursor = System.Windows.Input.Cursors.Hand
        };

        addGearButton.Click += (sender, e) =>
        {
            try
            {
                //New Gear
                double newGear = 1.0;

                // Punkt hinzufügen
                gears.Add(newGear);

                // EnginePoint erhöhen
                int currentGearAmount =
                    drivetrain["numberOfGear"]!.GetValue<int>();

                drivetrain["numberOfGear"] = currentGearAmount + 1;

                // Speichern
                SaveTDR();

                // Seite neu laden
                ShowGearbox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Fehler beim Neuladen",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        };

        ContentPanel.Children.Add(addGearButton);
    }

    // =========================
    // OTHER
    // =========================

    private void ShowOther()
    {
        ContentPanel.Children.Clear();

        // =========================
        // SOUND SETTING FRAME
        // =========================

        Border frame = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(22, 22, 24)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(45, 45, 47)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(15),
            Margin = new Thickness(0, 0, 0, 15),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        Grid grid = new Grid();

        grid.RowDefinitions.Add(
            new RowDefinition { Height = GridLength.Auto }
        );

        grid.RowDefinitions.Add(
            new RowDefinition { Height = GridLength.Auto }
        );

        grid.RowDefinitions.Add(
            new RowDefinition { Height = GridLength.Auto }
        );


        // =========================
        // TITLE
        // =========================

        TextBlock title = new TextBlock
        {
            Text = "ENGINE SOUND",
            Foreground = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            ),
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 8)
        };

        Grid.SetRow(title, 0);
        grid.Children.Add(title);


        // =========================
        // DROPDOWN
        // =========================

        ComboBox engineSoundDropdown = new ComboBox
        {
            Height = 42,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            FontSize = 14,
            Background = new SolidColorBrush(
                Color.FromRgb(30, 30, 32)
            ),
            Foreground = Brushes.White,
            BorderBrush = new SolidColorBrush(
                Color.FromRgb(55, 55, 58)
            ),
            BorderThickness = new Thickness(1),
            Margin = new Thickness(0, 0, 0, 10)
        };


        // =========================
        // SOUND OPTIONS
        // =========================

        engineSoundDropdown.Items.Add("MX1252t");
        engineSoundDropdown.Items.Add("MX2502t");
        engineSoundDropdown.Items.Add("5002T");
        engineSoundDropdown.Items.Add("KW2504t");
        engineSoundDropdown.Items.Add("300");
        engineSoundDropdown.Items.Add("H4504t");
        engineSoundDropdown.Items.Add("Y600R");
        engineSoundDropdown.Items.Add("HTW750");
        engineSoundDropdown.Items.Add("Y1000R");
        engineSoundDropdown.Items.Add("H1000RR");
        engineSoundDropdown.Items.Add("G1300");
        engineSoundDropdown.Items.Add("1300");


        // =========================
        // LOAD SAVED SOUND
        // =========================

        string savedSound =
            tdr["EngineSound"]?.GetValue<string>()
            ?? "MX1252t";

        if (engineSoundDropdown.Items.Contains(savedSound))
        {
            engineSoundDropdown.SelectedItem = savedSound;
        }
        else
        {
            engineSoundDropdown.Items.Add(savedSound);
            engineSoundDropdown.SelectedItem = savedSound;
        }


        // =========================
        // ITEM STYLE
        // =========================

        Style itemStyle = new Style(typeof(ComboBoxItem));

        itemStyle.Setters.Add(new Setter(
            Control.BackgroundProperty,
            new SolidColorBrush(Color.FromRgb(30, 30, 32))
        ));

        itemStyle.Setters.Add(new Setter(
            Control.ForegroundProperty,
            Brushes.White
        ));

        itemStyle.Setters.Add(new Setter(
            Control.PaddingProperty,
            new Thickness(12, 8, 12, 8)
        ));

        engineSoundDropdown.ItemContainerStyle = itemStyle;


        // =========================
        // CUSTOM TEMPLATE
        // =========================

        string templateXaml = @"
    <ControlTemplate
        xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
        TargetType='ComboBox'>

        <Grid>

            <Border
                Background='#1E1E20'
                BorderBrush='#37373A'
                BorderThickness='1'
                CornerRadius='8'>

                <Grid>

                    <ContentPresenter
                        Content='{TemplateBinding SelectionBoxItem}'
                        VerticalAlignment='Center'
                        Margin='12,0,40,0'
                        TextElement.Foreground='White' />

                    <TextBlock
                        Text='▼'
                        Foreground='#FFD21C'
                        FontSize='10'
                        HorizontalAlignment='Right'
                        VerticalAlignment='Center'
                        Margin='0,0,14,0' />

                </Grid>

            </Border>

            <ToggleButton
                Background='Transparent'
                BorderThickness='0'
                IsChecked='{Binding IsDropDownOpen, RelativeSource={RelativeSource TemplatedParent}}'
                Focusable='False' />

            <Popup
                x:Name='PART_Popup'
                IsOpen='{TemplateBinding IsDropDownOpen}'
                Placement='Bottom'
                AllowsTransparency='True'
                Focusable='False'
                HorizontalOffset='0'>

                <Border
                    Width='400'
                    Background='#1E1E20'
                    BorderBrush='#37373A'
                    BorderThickness='1'
                    CornerRadius='8'
                    Margin='0,4,0,0'>

                    <ScrollViewer
                        MaxHeight='250'
                        CanContentScroll='True'>

                        <ItemsPresenter />

                    </ScrollViewer>

                </Border>

            </Popup>

        </Grid>

    </ControlTemplate>";

        engineSoundDropdown.Template =
            (ControlTemplate)XamlReader.Parse(templateXaml);

        Grid.SetRow(engineSoundDropdown, 1);
        grid.Children.Add(engineSoundDropdown);


        // =========================
        // SAVE BUTTON
        // =========================

        Button saveButton = new Button
        {
            Content = "SAVE",
            Width = 90,
            Height = 36,
            HorizontalAlignment = HorizontalAlignment.Left,
            Background = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            ),
            Foreground = new SolidColorBrush(
                Color.FromRgb(11, 11, 12)
            ),
            BorderThickness = new Thickness(0),
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Cursor = System.Windows.Input.Cursors.Hand
        };

        Grid.SetRow(saveButton, 2);
        grid.Children.Add(saveButton);


        // =========================
        // SAVE CLICK
        // =========================

        saveButton.Click += async (sender, e) =>
        {
            if (engineSoundDropdown.SelectedItem == null)
                return;

            string selectedSound =
                engineSoundDropdown.SelectedItem.ToString();

            tdr["EngineSound"] = selectedSound;

            SaveTDR();

            saveButton.Content = "✓ SAVED";

            saveButton.Background = new SolidColorBrush(
                Color.FromRgb(60, 130, 70)
            );

            saveButton.Foreground = Brushes.White;

            await Task.Delay(1000);

            saveButton.Content = "SAVE";

            saveButton.Background = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            );

            saveButton.Foreground = new SolidColorBrush(
                Color.FromRgb(11, 11, 12)
            );
        };


        frame.Child = grid;

        ContentPanel.Children.Add(frame);


        // =====================================================
        // BIKE NAME FRAME
        // =====================================================

        Border bikeNameFrame = new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(22, 22, 24)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(45, 45, 47)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(15),
            Margin = new Thickness(0, 0, 0, 15),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        Grid bikeNameGrid = new Grid();

        bikeNameGrid.RowDefinitions.Add(
            new RowDefinition { Height = GridLength.Auto }
        );

        bikeNameGrid.RowDefinitions.Add(
            new RowDefinition { Height = GridLength.Auto }
        );

        bikeNameGrid.RowDefinitions.Add(
            new RowDefinition { Height = GridLength.Auto }
        );


        // =========================
        // TITLE
        // =========================

        TextBlock bikeNameTitle = new TextBlock
        {
            Text = "INGAME BIKE NAME",
            Foreground = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            ),
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 8)
        };

        Grid.SetRow(bikeNameTitle, 0);
        bikeNameGrid.Children.Add(bikeNameTitle);


        // =========================
        // INPUT
        // =========================

        TextBox bikeNameInput = new TextBox
        {
            Text = tdr["BikeName"]?.GetValue<string>() ?? "",
            Height = 42,
            FontSize = 14,
            Foreground = Brushes.White,
            Background = new SolidColorBrush(
                Color.FromRgb(30, 30, 32)
            ),
            BorderBrush = new SolidColorBrush(
                Color.FromRgb(55, 55, 58)
            ),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(12, 0, 12, 0),
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CaretBrush = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            )
        };

        Grid.SetRow(bikeNameInput, 1);
        bikeNameGrid.Children.Add(bikeNameInput);


        // =========================
        // SAVE BUTTON
        // =========================

        Button saveBikeNameButton = new Button
        {
            Content = "SAVE",
            Width = 90,
            Height = 36,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 10, 0, 0),

            Background = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            ),

            Foreground = new SolidColorBrush(
                Color.FromRgb(11, 11, 12)
            ),

            BorderThickness = new Thickness(0),
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Cursor = System.Windows.Input.Cursors.Hand
        };

        Grid.SetRow(saveBikeNameButton, 2);
        bikeNameGrid.Children.Add(saveBikeNameButton);


        // =========================
        // SAVE CLICK
        // =========================

        saveBikeNameButton.Click += async (sender, e) =>
        {
            string newBikeName = bikeNameInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(newBikeName))
            {
                MessageBox.Show(
                    "Please enter a valid bike name.",
                    "Invalid Bike Name",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                return;
            }

            tdr["BikeName"] = newBikeName;

            SaveTDR();


            // =========================
            // SAVE EFFECT
            // =========================

            saveBikeNameButton.Content = "✓ SAVED";

            saveBikeNameButton.Background = new SolidColorBrush(
                Color.FromRgb(60, 130, 70)
            );

            saveBikeNameButton.Foreground = Brushes.White;


            await Task.Delay(1000);


            saveBikeNameButton.Content = "SAVE";

            saveBikeNameButton.Background = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            );

            saveBikeNameButton.Foreground = new SolidColorBrush(
                Color.FromRgb(11, 11, 12)
            );
        };


        bikeNameFrame.Child = bikeNameGrid;

        ContentPanel.Children.Add(bikeNameFrame);
    }

    // =========================
    // LOAD / SAVE MODIFICATION
    // =========================

    private void ShowLoadSaveModification()
    {
        ContentPanel.Children.Clear();

        // =========================
        // FRAME
        // =========================

        Border frame = new Border
        {
            Background = new SolidColorBrush(
                Color.FromRgb(22, 22, 24)
            ),
            BorderBrush = new SolidColorBrush(
                Color.FromRgb(45, 45, 47)
            ),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(15),
            Margin = new Thickness(0, 0, 0, 15),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        Grid grid = new Grid();

        grid.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            }
        );

        grid.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            }
        );


        // =========================
        // TITLE
        // =========================

        TextBlock title = new TextBlock
        {
            Text = "MODIFICATION",
            Foreground = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            ),
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 12)
        };

        Grid.SetColumn(title, 0);
        Grid.SetColumnSpan(title, 2);


        // =========================
        // SAVE BUTTON
        // =========================

        Button saveButton = new Button
        {
            Content = "SAVE MODIFICATION",
            Height = 50,
            Margin = new Thickness(0, 25, 7, 0),

            Background = new SolidColorBrush(
                Color.FromRgb(255, 210, 28)
            ),

            Foreground = new SolidColorBrush(
                Color.FromRgb(11, 11, 12)
            ),

            BorderThickness = new Thickness(0),
            FontSize = 12,
            FontWeight = FontWeights.Bold,

            Cursor = System.Windows.Input.Cursors.Hand
        };


        // =========================
        // LOAD BUTTON
        // =========================

        Button loadButton = new Button
        {
            Content = "LOAD MODIFICATION",
            Height = 50,
            Margin = new Thickness(7, 25, 0, 0),

            Background = new SolidColorBrush(
                Color.FromRgb(35, 35, 38)
            ),

            Foreground = Brushes.White,

            BorderBrush = new SolidColorBrush(
                Color.FromRgb(55, 55, 58)
            ),

            BorderThickness = new Thickness(1),
            FontSize = 12,
            FontWeight = FontWeights.Bold,

            Cursor = System.Windows.Input.Cursors.Hand
        };


        // =========================
        // SAVE CLICK
        // =========================

        saveButton.Click += (sender, e) =>
        {
            ModificationManager.SaveModification(tdrFile);
        };


        // =========================
        // LOAD CLICK
        // =========================

        loadButton.Click += (sender, e) =>
        {
            FileInfo? loadedTDR = ModificationManager.LoadModification();

            if (loadedTDR == null)
                return;


            try
            {
                // Modification laden
                JsonObject loadedJson =
                    JsonNode.Parse(
                        File.ReadAllText(loadedTDR.FullName)
                    )!.AsObject();


                // =========================
                // DRIVETRAIN ÜBERNEHMEN
                // =========================

                if (loadedJson["drivetrain"] != null)
                {
                    tdr["drivetrain"] =
                        JsonNode.Parse(
                            loadedJson["drivetrain"]!.ToJsonString()
                        );
                }


                // =========================
                // ENGINE SOUND ÜBERNEHMEN
                // =========================

                if (loadedJson["EngineSound"] != null)
                {
                    tdr["EngineSound"] =
                        loadedJson["EngineSound"]!.GetValue<string>();
                }

                // =========================
                // NAME ÜBERNEHMEN
                // =========================

                if (loadedJson["BikeName"] != null)
                {
                    tdr["BikeName"] =
                        loadedJson["BikeName"]!.GetValue<string>();
                }

                // =========================
                // AKTUELLE TDR SPEICHERN
                // =========================

                SaveTDR();


                MessageBox.Show(
                    "Modification loaded successfully!",
                    "Loaded",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );


                // =========================
                // FENSTER NEU LADEN
                // =========================

                ModifyBikeWindow window =
                    new ModifyBikeWindow(bike);

                window.Show();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not load modification:\n\n{ex.Message}",
                    "Load Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        };

        // =========================
        // GRID POSITION
        // =========================

        Grid.SetRow(title, 0);

        Grid.SetColumn(saveButton, 0);
        Grid.SetColumn(loadButton, 1);


        // =========================
        // ADD ELEMENTS
        // =========================

        grid.Children.Add(title);
        grid.Children.Add(saveButton);
        grid.Children.Add(loadButton);

        frame.Child = grid;

        ContentPanel.Children.Add(frame);
    }
}