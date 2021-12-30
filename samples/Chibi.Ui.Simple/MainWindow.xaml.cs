using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Chibi.Ui.MicroGraphics;
using Meadow.Foundation.Graphics;
using Color = Meadow.Foundation.Color;

namespace Chibi.Ui.Simple;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly WriteableBitmap _canvas = new(128, 64, 96, 96, PixelFormats.Bgr24, null);
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
        _renderingContext = new RenderingContext(_graphics);
        Task.Factory.StartNew(RenderLoop, _exitRenderLoop, TaskCreationOptions.LongRunning);
    }

    private async Task RenderLoop(object? _)
    {
        await Initialize();

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

    private Task Initialize()
    {
        _screen = new MainMenuScreen();
        return Task.CompletedTask;
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
}