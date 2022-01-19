using System;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;

namespace Chibi.Ui.MicroGraphics
{
    public sealed class RenderingContext
    {
        private readonly Meadow.Foundation.Graphics.MicroGraphics _graphics;

        public RenderingContext(Meadow.Foundation.Graphics.MicroGraphics graphics, RenderingArea area)
        {
            Area = area;
            _graphics = graphics;
        }

        public RenderingContext(Meadow.Foundation.Graphics.MicroGraphics graphics)
            : this(graphics, new RenderingArea(0, 0, Math.Abs(graphics.Width), Math.Abs(graphics.Height)))
        {
        }

        public RenderingArea Area { get; }

        public IFont DefaultFont { get; set; } = new Font8x8();

        public RenderingContext Create(RenderingArea rect)
        {
            return new RenderingContext(_graphics, rect)
            {
                DefaultFont = DefaultFont
            };
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
            IFont? font = null,
            TextAlignment alignment = TextAlignment.Left)
        {
            var actualX = Area.X + x;
            var actualY = Area.Y + y;

            font ??= DefaultFont;

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

        public void DrawTriangle(Point p0, Point p1, Point p2, bool colored = true, bool filled = false)
        {
            _graphics.DrawTriangle(Area.X + p0.X, Area.Y + p0.Y, Area.X + p1.X, Area.Y + p1.Y, Area.X + p2.X,
                Area.Y + p2.Y, colored, filled);
        }

        public void DrawLine(Point p0, Point p1, bool colored = true)
        {
            _graphics.DrawLine(Area.X + p0.X, Area.Y + p0.Y, Area.X + p1.X, Area.Y + p1.Y, colored);
        }

        public void DrawBuffer(int x, int y, IDisplayBuffer buffer)
        {
            _graphics.DrawBuffer(Area.X + x, Area.Y + y, buffer);
        }
    }
}