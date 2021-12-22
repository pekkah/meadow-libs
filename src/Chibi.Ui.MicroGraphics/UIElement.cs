using System;

namespace Chibi.Ui.MicroGraphics
{
    public abstract class UiElement : Renderable, IHasSize
    {
        protected UiElement(
            Func<Length>? width = null,
            Func<Length>? height = null)
        {
            Width = width ?? (() => Length.Auto);
            Height = height ?? (() => Length.Auto);
        }

        public Func<Length> Width { get; }

        public Func<Length> Height { get; }
    }
}