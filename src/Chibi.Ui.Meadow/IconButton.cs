using System;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.Meadow
{
    public class IconButton : UiElement
    {
        private readonly Func<Icon> _icon;
        private readonly MarginRenderable _marginRenderable;
        private readonly MarginRenderable _paddingRenderable;
        private readonly Func<string> _text;

        public IconButton(
            Func<Icon>? icon = null,
            Func<string>? text = null,
            Func<Length>? width = null,
            Func<Length>? height = null,
            Func<Margin>? margin = null,
            Func<Margin>? padding = null)
            : base(width, height)
        {
            _icon = icon ?? (() => Icon.Play);
            _text = text ?? (() => string.Empty);
            _marginRenderable = new MarginRenderable(margin ?? (() => Margin.Zero), RenderWithMargin);
            _paddingRenderable = new MarginRenderable(padding ?? (() => Margin.Zero), RenderWithPadding);
        }

        public override void Render(RenderingContext context)
        {
            _marginRenderable.Render(context);
        }

        private void RenderWithMargin(RenderingContext context)
        {
            context.DrawRectangle(0, 0, context.Area.Width, context.Area.Height);
            _paddingRenderable.Render(context);
        }

        private void RenderWithPadding(RenderingContext context)
        {
            //todo: use VerticalLayout
            var iconContext = context.Create(10, 6, context.Area.Width - (10*2), context.Area.Height / 2-4);
            _icon().Render(iconContext);

            /*context.DrawCircle(
                context.Area.Width / 2,
                context.Area.Height / 2 - 6,
                6,
                true);*/

            context.DrawText(
                context.Area.Width / 2,
                context.Area.Height - 8 - 4,
                _text(),
                new Font4x8(),
                TextAlignment.Center);
        }
    }
}