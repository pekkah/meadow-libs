using System;
using System.Collections.Generic;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.MicroGraphics
{
    public class IconButton : UiElement
    {
        private readonly Func<Icon> _icon;
        private readonly MarginRenderable _paddingRenderable;
        private readonly Func<string> _text;
        private readonly VerticalLayout _layout;

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
            _paddingRenderable = new MarginRenderable(padding ?? (() => Margin.Two), RenderWithPadding);
            _layout = new VerticalLayout(Children(), margin);
        }

        private IEnumerable<Renderable> Children()
        {
            yield return _icon();
            yield return new Text(_text, () => TextAlignment.Center, height: ()=> 22);
        }

        public override void Render(RenderingContext context)
        {
            _paddingRenderable.Render(context);
        }

        private void RenderWithPadding(RenderingContext context)
        {
            context.DrawRectangle(0,0, context.Area.Width, context.Area.Height);
            _layout.Render(context);
        }
    }
}