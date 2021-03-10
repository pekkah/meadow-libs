using System;
using Chibi.Infrastructure.Logging;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.Meadow
{
    public sealed class RenderingContext
    {
        private static readonly Logger Logger = Logger.GetLogger(nameof(RenderingContext));
        private readonly GraphicsLibrary _graphics;

        public RenderingContext(GraphicsLibrary graphics, RenderingArea area)
        {
            Area = area;
            _graphics = graphics;
        }

        public RenderingContext(GraphicsLibrary graphics)
            : this(graphics, new RenderingArea(0, 0, Math.Abs(graphics.Width), Math.Abs(graphics.Height)))
        {
        }

        public FontBase DefaultFont { get; } = new Font8x8();

        public RenderingArea Area { get; }


        public RenderingContext Create(RenderingArea rect)
        {
            return new RenderingContext(_graphics, rect);
        }

        public RenderingContext Create()
        {
            return Create(Area);
        }

        public RenderingContext Create(int x, int y, int width, int height)
        {
            return Create(new RenderingArea(Area.X + x, Area.Y + y, width, height));
        }

        public void DrawRectangle(int x, int y, int width, int height)
        {
            var actualX = Area.X + x;
            var actualY = Area.Y + y;
            _graphics.DrawRectangle(actualX, actualY, width, height);
        }

        public void DrawText(
            int x,
            int y,
            string text,
            FontBase font = null,
            GraphicsLibrary.TextAlignment alignment = GraphicsLibrary.TextAlignment.Left)
        {
            var actualX = Area.X + x;
            var actualY = Area.Y + y;

            if (font == null)
                font = DefaultFont;

            var previousFont = _graphics.CurrentFont;
            _graphics.CurrentFont = font;

            _graphics.DrawText(
                actualX,
                actualY,
                text,
                alignment: alignment);

            _graphics.CurrentFont = previousFont;
        }

        public void DrawCircle(int x, int y, int radius, bool filled)
        {
            var actualX = Area.X + x;
            var actualY = Area.Y + y;

            _graphics.DrawCircle(actualX, actualY, radius, filled: filled);
        }
    }
}