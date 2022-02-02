using System;
using Chibi.Ui.MicroGraphics;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Serilog;

namespace Chibi.Ui.Simple.Screens
{

    public class MainMenuScreen
    {
        private readonly Renderable _renderable;

        private int _currentButtonId = 0;
        private readonly Icon _play;
        private readonly Icon _settings;
        private readonly Icon _test;
        private static readonly ILogger Logger = Log.ForContext<MainMenuScreen>();
        private Icons _icons = new();

        public MainMenuScreen(int width, int height)
        {
            _play = LoadIcon("play", width, height);
            _settings = LoadIcon("settings", width, height);
            _test = LoadIcon("test", width, height);
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
                                icon: () => _play,
                                padding: () => CurrentButtonMargin(0),
                                text: () => "Button 1"
                            ),
                            new IconButton(
                                icon: () => _test,
                                padding: () => CurrentButtonMargin(1),
                                text: () => "Button 2"
                            ),
                            new IconButton(
                                icon: () => _settings,
                                padding: () => CurrentButtonMargin(2),
                                text: () => "Button 3"
                            )
                        })
                });
        }

        private Icon LoadIcon(string name, int screenWidth, int screenHeight)
        {
            IDisplayBuffer buffer = (name, screenWidth) switch
            {
                ("play", <= 128) => _icons.Play16x16,
                ("settings", <= 128) => _icons.Settings16x16,
                ("test", <= 128) => _icons.Test16x16,
                ("play", _) => _icons.Play32x32,
                ("settings", _) => _icons.Settings32x32,
                ("test", _) => _icons.Test32x32,
                _ => throw new ArgumentOutOfRangeException(nameof(name))
            };

            return new Icon(context =>
            {
                // center image
                var mX = context.Area.Width / 2;
                var offsetX = Math.Max(0, mX - (buffer.Width / 2));

                var mY = context.Area.Height / 2;
                var offsetY = Math.Max(0, mY - (buffer.Height / 2));

                context.DrawBuffer(offsetX, offsetY, buffer);
            }, () => buffer.Width, () => buffer.Height);
        }

        public void Render(RenderingContext context)
        {
            _renderable.Render(context);
        }

        private Margin CurrentButtonMargin(int buttonId)
        {
            return buttonId == _currentButtonId ? Margin.Zero : new Margin(0, 4, 0, 2);
        }

        public void Left()
        {
            if (_currentButtonId == 0)
                return;

            _currentButtonId--;
        }

        public void Right()
        {
            if (_currentButtonId == 2)
                return;

            _currentButtonId++;
        }
    }
}