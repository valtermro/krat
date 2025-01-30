using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Themes.Simple;

namespace Krat;

// TODO: Testar
// - Com monitores em outros lados do principal
// - Com monitores em outras escalas

// TODO: UX
// - Destacar o texto digitado até o momento para o "hint" acumulado.

public class Program : Application
{
    [STAThread]
    public static void Main(string[] args)
    {
#if DEBUG
        // TODO: Mover para testes
        var hints = HintGenerator.New().ToArray();
        foreach (var hint in hints)
        {
            if (Array.Exists(hints, p => p != hint && (p.StartsWith(hint) || hint.StartsWith(p))))
            {
                throw new Exception("Hint collision detected.");
            }
        }
#endif

        AppBuilder
            .Configure<Program>()
            .UsePlatformDetect()
            .LogToTrace()
            .StartWithClassicDesktopLifetime(args, ShutdownMode.OnExplicitShutdown);
    }

    private Win32Keyboard _keyboard = null!;
    private ClassicDesktopStyleApplicationLifetime _lifetime = null!;

    private sealed record HintPoint(Screen Screen, Label Text);

    private bool _hintActive = false;
    private readonly List<Window> _hintWindows = [];
    private readonly Dictionary<string, HintPoint> _hints = [];
    private readonly List<char> _hintTest = [];

    public override void Initialize()
    {
        base.Initialize();
        _lifetime = (ClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
        _keyboard = new Win32Keyboard();

        _keyboard.Initialize(OnKeyInput);
        _lifetime.ShutdownRequested += (_, _) => _keyboard.Shutdown();

        // Application needs a theme to render window content
        Styles.Add(new SimpleTheme());
        RequestedThemeVariant = ThemeVariant.Default; // Default, Dark, Light

        Run();
    }

    private void Run()
    {
        var primaryWindow = new Window();
        var screens = primaryWindow.Screens;
        using var hintEnumerator = HintGenerator.New().GetEnumerator();

        foreach (var screen in screens.All.OrderBy(p => p.IsPrimary ? 0 : 1))
        {
            var screenWidth = screen.Bounds.Width;
            var screenHeight = screen.Bounds.Height;

            var window = screen.IsPrimary ? primaryWindow : new Window();
            _hintWindows.Add(window);

            window.WindowState = WindowState.FullScreen;
            window.Background = new SolidColorBrush(Colors.Transparent);
            window.Topmost = true;
            window.Position = new PixelPoint(screen.Bounds.X, screen.Bounds.Y);
            window.ShowInTaskbar = false;

            var grid = new Grid();
            window.Content = grid;

            const double cellWidth = 68;
            const double cellHeight = 48;
            var gridColumns = (int)(screenWidth / cellWidth / screen.Scaling);
            var gridRows = (int)(screenHeight / cellHeight / screen.Scaling);

            for (var row = 0; row < gridRows; row++)
                grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star));

            for (var col = 0; col < gridColumns; col++)
                grid.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star));

            for (var row = 0; row < gridRows; row++)
            {
                for (var col = 0; col < gridColumns; col++)
                {
                    if (!hintEnumerator.MoveNext())
                    {
                        // TODO: Algo que apareça para o usuário
                        throw new Exception("Hint generation failed.");
                    }

                    var text = new Label();
                    grid.Children.Add(text);

                    text.Background = new SolidColorBrush(Colors.Black);
                    text.Foreground = new SolidColorBrush(Colors.White);
                    text.HorizontalContentAlignment = HorizontalAlignment.Center;
                    text.VerticalContentAlignment = VerticalAlignment.Center;
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.FontSize = 14;

                    text.SetValue(Grid.ColumnProperty, col);
                    text.SetValue(Grid.RowProperty, row);

                    var hint = hintEnumerator.Current;
                    _hints[hint] = new HintPoint(screen, text);
                    text.Content = hint;
                }
            }
        }
    }

    private void ShowHintWindows()
    {
        foreach (var window in _hintWindows)
        {
            window.Show();
            _hintActive = true;
        }
    }

    private void HideHintWindows()
    {
        foreach (var window in _hintWindows)
        {
            window.Hide();
            _hintActive = false;
        }

        _hintTest.Clear();
    }

    private bool OnKeyInput(KeyEvent e)
    {
        if (e.Kind != KeyEventKind.KeyDown)
            return true;

        if (e.Key == KeyCode.Escape && _hintActive)
        {
            if (_hintTest.Count != 0)
                _hintTest.Clear();
            else
                HideHintWindows();
            return false;
        }

        if (e.Key == KeyCode.F6 && e.HasMod(KeyModifier.Control))
        {
            ShowHintWindows();
            return false;
        }

        if (!_hintActive)
            return true;

        if (!e.IsAlphaKey())
        {
            _hintTest.Clear();
            return false;
        }

        _hintTest.Add(e.KeyChar());
        HintPoint? match = null;

        foreach (var hint in _hints)
        {
            if (hint.Key.Length != _hintTest.Count)
                continue;

            var matchCount = 0;

            for (var i = 0; i < hint.Key.Length; i++)
            {
                if (_hintTest[i] != hint.Key[i])
                    break;
                matchCount += 1;
            }

            if (matchCount == hint.Key.Length)
            {
                match = hint.Value;
                break;
            }
        }

        if (match != null)
        {
            var x = (int)(match.Text.Bounds.X * match.Screen.Scaling + match.Text.Bounds.Width / 2) + match.Screen.Bounds.X;
            var y = (int)(match.Text.Bounds.Y * match.Screen.Scaling + match.Text.Bounds.Height / 2) + match.Screen.Bounds.Y;

            Win32Mouse.SetCursorPos(x, y);
            HideHintWindows();
        }

        return false;
    }
}