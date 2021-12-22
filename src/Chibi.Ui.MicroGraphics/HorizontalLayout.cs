using System;
using System.Collections.Generic;
using System.Linq;

namespace Chibi.Ui.MicroGraphics
{
    public class HorizontalLayout : Renderable, IHasSize
    {
        private readonly List<Renderable> _children;

        public HorizontalLayout(
            IEnumerable<Renderable> children,
            Func<Margin>? margin = null,
            Func<Length>? width = null,
            Func<Length>? height = null)
        {
            _children = children.ToList();
            MarginRenderable = new MarginRenderable(margin ?? (() => Margin.Zero), RenderChildren);
            Width = width ?? (() => Length.Auto);
            Height = height ?? (() => Length.Auto);
        }

        protected MarginRenderable MarginRenderable { get; }

        public Func<Length> Width { get; }

        public Func<Length> Height { get; }

        public override void Render(RenderingContext context)
        {
            MarginRenderable.Render(context);
        }

        protected virtual void RenderChildren(RenderingContext context)
        {
            var width = context.Area.Width;
            var height = context.Area.Height;

            // calculate total requested height
            var hasSizeCount = 0;
            var totalRequestedWidth = _children
                .OfType<IHasSize>()
                .Sum(child =>
                {
                    var requestedWidth = child.Width();

                    if (requestedWidth.IsAuto)
                        return 0;

                    hasSizeCount++;
                    return requestedWidth;
                });

            if (totalRequestedWidth > width)
                throw new InvalidOperationException(
                    $"Total requested width {totalRequestedWidth} of children is larger than available space {width}");

            var autoChildWidth = (width - totalRequestedWidth) / (_children.Count - hasSizeCount);

            // do rendering
            var childX = 0;
            var childY = 0;

            foreach (var child in _children)
            {
                var childHeight = height;
                var childWidth = autoChildWidth;

                if (child is IHasSize hasSize)
                {
                    var requestedWidth = hasSize.Width();
                    if (!requestedWidth.IsAuto) childWidth = requestedWidth;

                    var requestedHeight = hasSize.Height();

                    if (!requestedHeight.IsAuto && requestedHeight < width)
                        childHeight = requestedHeight;
                }

                child.Render(context.Create(childX, childY, childWidth, childHeight));
                childX += childWidth;
            }
        }
    }
}