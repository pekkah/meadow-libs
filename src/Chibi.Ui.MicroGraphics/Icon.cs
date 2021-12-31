using System;
using System.Runtime.InteropServices.ComTypes;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.MicroGraphics
{
    public class Icon : UiElement
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
        private readonly MarginRenderable _margin;
        private readonly MarginRenderable _padding;

        public Icon(
            Action<RenderingContext> contents,
            Func<Length>? width = null,
            Func<Length>? height = null,
            Func<Margin>? margin = null,
            Func<Margin>? padding = null) : base(width, height)
        {
            _contents = contents;
            _margin = new MarginRenderable(margin ?? (() => new Margin(0, 0, 0, 0)), RenderWithMargin);
            _padding = new MarginRenderable(padding ?? (() => new Margin(2, 2, 2, 2)), RenderWithPadding);
        }

        private void RenderWithPadding(RenderingContext context)
        {
            _contents(context);
        }

        private void RenderWithMargin(RenderingContext context)
        {   
            _padding.Render(context);
        }

        public override void Render(RenderingContext context)
        {
            _margin.Render(context);
        }
    }
}