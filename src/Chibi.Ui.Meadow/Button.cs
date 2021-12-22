using System;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.Meadow
{
    public class Button : UiElement
    {
        private readonly MarginRenderable _marginRenderable;
        private readonly MarginRenderable _paddingRenderable;

        public Button(
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
            context.DrawText(
                context.Area.Width / 2,
                context.Area.Height / 2,
                Text(),
                new Font4x8(),
                TextAlignment.Center);
        }

        public override void Render(RenderingContext context)
        {
            _marginRenderable.Render(context);
        }
    }
}