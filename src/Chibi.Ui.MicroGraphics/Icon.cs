using System;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.MicroGraphics
{
    public class Icon : Renderable
    {
        public static Icon Play = new(context =>
        {
            context.DrawTriangle(new Point(0), new Point(context.Area.Width, context.Area.Height / 2),
                new Point(0, context.Area.Height));
        });

        public static Icon Settings = new(context =>
        {
            var start = new Point(4, 2);
            var end = new Point(context.Area.Width - 8, context.Area.Height);
            context.DrawLine(start, end);
            context.DrawCircle(start.X, start.Y, 2, true);
            context.DrawCircle(end.X, end.Y, 1, true);
        });

        private readonly Action<RenderingContext> _contents;

        public Icon(Action<RenderingContext> contents)
        {
            _contents = contents;
        }

        public override void Render(RenderingContext context)
        {
            _contents(context);
        }
    }
}