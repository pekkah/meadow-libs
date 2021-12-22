using System;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui.Meadow
{
    public class Text : UiElement
    {
        private readonly MarginRenderable _marginRenderable;
        private readonly MarginRenderable _paddingRenderable;

        public Text(
            Func<string> value,
            Func<TextAlignment>? alignment = null,
            Func<IFont>? font = null,
            Func<Length>? width = null,
            Func<Length>? height = null,
            Func<Margin>? margin = null,
            Func<Margin>? padding = null)
            : base(width, height)
        {
            Value = value;
            Font = font;
            Alignment = alignment ?? (() => TextAlignment.Left);
            _marginRenderable = new MarginRenderable(margin ?? (() => Margin.Zero), RenderWithMargin);
            _paddingRenderable = new MarginRenderable(padding ?? (() => Margin.Zero), RenderWithPadding);
        }

        public Func<TextAlignment> Alignment { get; set; }

        public Func<string> Value { get; }

        public Func<IFont>? Font { get; }

        private void RenderWithMargin(RenderingContext context)
        {
            _paddingRenderable.Render(context);
        }

        private void RenderWithPadding(RenderingContext context)
        {
            var font = Font != null ? Font() : context.DefaultFont;
            var alignment = Alignment();


            switch (alignment)
            {
                case TextAlignment.Left:
                    context.DrawText(
                        0,
                        0,
                        Value(),
                        font,
                        alignment);
                    break;
                case TextAlignment.Center:
                    context.DrawText(
                        context.Area.Width / 2,
                        0,
                        Value(),
                        font,
                        alignment);
                    break;
                case TextAlignment.Right:
                    context.DrawText(
                        context.Area.Width,
                        0,
                        Value(),
                        font,
                        alignment);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Render(RenderingContext context)
        {
            _marginRenderable.Render(context);
        }
    }
}