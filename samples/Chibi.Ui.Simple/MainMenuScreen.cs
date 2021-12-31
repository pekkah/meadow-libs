using System;
using System.IO;
using System.Runtime.InteropServices;
using Chibi.Ui.MicroGraphics;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Chibi.Ui.Simple;

public class MainMenuScreen
{
    private readonly Renderable _renderable;

    private int _currentButtonId = 0;
    private readonly Icon _play;
    private readonly Icon _settings;
    private readonly Icon _test;

    public MainMenuScreen()
    {
        _play = LoadIcon("icons/play.png");
        _settings = LoadIcon("icons/settings.png");
        _test = LoadIcon("icons/test.png");
        _renderable = new VerticalLayout(
            new[]
            {
                new HorizontalLayout(
                    height: () => 16,
                    margin: () => new Margin(2, 2, 2, 2),
                    children: new[]
                    {
                        new Text(
                            () => "MENU",
                            () => TextAlignment.Center
                        )
                    }),
                new HorizontalLayout(
                    new[]
                    {
                        new IconButton(
                            icon: ()=> _play,
                            padding: () => CurrentButtonMargin(0),
                            text: () => "Button 1"
                        ),
                        new IconButton(
                            icon: ()=> _test,
                            padding: () => CurrentButtonMargin(1),
                            text: () => "Button 2"
                        ),
                        new IconButton(
                            icon: ()=> _settings,
                            padding: () => CurrentButtonMargin(2),
                            text: () => "Button 3"
                        )
                    })
            });
    }

    private Icon LoadIcon(string path)
    {
        try
        {
            using var image = Image.Load(File.ReadAllBytes(path));
            using var bgr = image.CloneAs<Bgr24>();
            if (bgr.TryGetSinglePixelSpan(out var bgrSpan))
            {
                var span = MemoryMarshal.AsBytes(bgrSpan);
                var bytes = span.ToArray();


                var buffer = new BufferRgb888(bgr.Width, bgr.Height, bytes);

                return new Icon(context =>
                {
                    // center image
                    var mX = context.Area.Width / 2;
                    var offsetX = Math.Max(0, mX - (buffer.Width / 2));

                    var mY = context.Area.Height / 2;
                    var offsetY = Math.Max(0, mY - (buffer.Height / 2));

                    context.DrawBuffer(offsetX, offsetY, buffer);
                }, () => Length.Auto, () => Length.Auto);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        catch (Exception x)
        {
            throw;
        }
    }

    public void Render(RenderingContext context)
    {
        _renderable.Render(context);
    }

    private Margin CurrentButtonMargin(int buttonId)
    {
        return buttonId == _currentButtonId ? Margin.Zero : new Margin(0, 4, 0, 2);
    }
}