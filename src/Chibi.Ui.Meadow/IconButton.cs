using System;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.Meadow
{
    public class IconButton : UiElement
    {
        private readonly MarginRenderable _marginRenderable;
        private readonly MarginRenderable _paddingRenderable;

        public IconButton(
            Func<string> text = null,
            Func<Length> width = null,
            Func<Length> height = null,
            Func<Margin> margin = null,
            Func<Margin> padding = null)
            : base(width, height)
        {
            Text = text ?? (() => string.Empty);
            _marginRenderable = new MarginRenderable(margin ?? (() => Margin.Zero), RenderWithMargin);
            _paddingRenderable = new MarginRenderable(padding ?? (() => Margin.Zero), RenderWithPadding);
        }

        public Func<string> Text { get; }

        private void RenderWithMargin(RenderingContext context)
        {
            context.DrawRectangle(0, 0, context.Area.Width, context.Area.Height);
            _paddingRenderable.Render(context);
        }

        private void RenderWithPadding(RenderingContext context)
        {
            context.DrawCircle(
                context.Area.Width / 2,
                context.Area.Height / 2 - 6,
                6,
                true);

            context.DrawText(
                context.Area.Width / 2,
                context.Area.Height - 8 - 4,
                Text(),
                new Font4x8(),
                GraphicsLibrary.TextAlignment.Center);
        }

        public override void Render(RenderingContext context)
        {
            _marginRenderable.Render(context);
        }
    }
}