using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Chibi.Ui.MicroGraphics;
using Chibi.Ui.Simple.Screens;
using Meadow.Foundation.Graphics;
using Color = Meadow.Foundation.Color;

namespace Chibi.Ui.Simple;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly WriteableBitmap _canvas = new(512, 256, 120, 120, PixelFormats.Bgr24, null);
    private readonly WriteableBitmapDisplay _display;

    private readonly CancellationTokenSource _exitRenderLoop = new();

    private MainMenuScreen? _screen;
    private RenderingContext? _renderingContext;
    private Meadow.Foundation.Graphics.MicroGraphics _graphics;

    public MainWindow()
    {
        InitializeComponent();

        Display.Source = _canvas;

        _display = new WriteableBitmapDisplay(_canvas);
        
        _graphics = new Meadow.Foundation.Graphics.MicroGraphics(_display)
        {
            PenColor = Color.White,
            CurrentFont = new Font6x8()
        };
        _renderingContext = new RenderingContext(_graphics)
        {
            DefaultFont = new Font12x20()
        };
        _screen = new MainMenuScreen(_display.Width, _display.Height);
        Task.Factory.StartNew(RenderLoop, _exitRenderLoop, TaskCreationOptions.LongRunning);
    }

    private async Task RenderLoop(object? _)
    {
        // 30 fps
        long desiredFrametime = 1000 / 30;
        var stopwatch = new Stopwatch();
        while (!_exitRenderLoop.IsCancellationRequested)
        {
            stopwatch.Restart();
            await Draw();
            stopwatch.Stop();
            var actualFrametime = stopwatch.ElapsedMilliseconds;

            var delta = desiredFrametime - actualFrametime;
            if (delta > 0)
                await Task.Delay((int)delta);
        }
    }

    private Task Draw()
    {
        Dispatcher.Invoke(() =>
        {
            _display.Fill(Color.Black);
            _screen?.Render(_renderingContext ?? throw new InvalidOperationException());
            _graphics.Show();
        });
        return Task.CompletedTask;
    }

    private void Display_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Left:
                _screen?.Left();
                break;
            case Key.Right:
                _screen?.Right();
                break;
        }
    }
}