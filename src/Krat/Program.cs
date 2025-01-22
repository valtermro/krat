using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Themes.Simple;

namespace Krat;

public class Program : Application
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppBuilder
            .Configure<Program>()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
    }

    public override void Initialize()
    {
        base.Initialize();

        // Application needs a theme to render window content
        Styles.Add(new SimpleTheme());
        RequestedThemeVariant = ThemeVariant.Default; // Default, Dark, Light

        Run();
    }

    private sealed record MidPoint(double X, double Y);

    private void Run()
    {
        var primaryWindow = CreateWindow();
        var screens = primaryWindow.Screens;
        var hintCells = new Dictionary<string, MidPoint>();

        var windows = new List<Window>(screens.ScreenCount);

        foreach (var screen in screens.All)
        {
            var window = screen.IsPrimary ? primaryWindow : CreateWindow();
            windows.Add(window);

            var grid = new Grid();
            grid.ShowGridLines = true;

            window.Content = grid;

            const int cellWidth = 68;
            const int cellHeight = 48;
            var gridColumns = screen.Bounds.Width / cellWidth;
            var gridRows = screen.Bounds.Height / cellHeight;

            for (var row = 0; row < gridRows; row++)
                grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));

            for (var col = 0; col < gridColumns; col++)
                grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));

            for (var row = 0; row < gridRows; row++)
            {
                for (var col = 0; col < gridColumns; col++)
                {
                    var hint = $"{row + 1}x{col + 1}";

                    hintCells[hint] = new MidPoint(
                        (row + 1d) * cellWidth - cellWidth / 2d,
                        (col + 1d) * cellHeight - cellHeight / 2d);

                    var text = new Label();
                    text.SetValue(Grid.ColumnProperty, col);
                    text.SetValue(Grid.RowProperty, row);

                    text.Background = new SolidColorBrush(Colors.Black);
                    text.Foreground = new SolidColorBrush(Colors.White);
                    text.Width = cellWidth;
                    text.Height = cellHeight;

                    text.Content = hint;
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.FontSize = 14;

                    grid.Children.Add(text);
                }
            }
        }

        foreach (var window in windows)
        {
            window.Show();
        }
    }

    private static Window CreateWindow()
    {
        var window = new Window();
        window.WindowState = WindowState.FullScreen;
        window.Background = new SolidColorBrush(Colors.Black, 0.25);
        //window.Topmost = true;
        return window;
    }
}