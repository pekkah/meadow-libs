using System;
using System.Collections.Generic;
using System.Linq;

namespace Chibi.Ui.Meadow
{
    public class VerticalLayout : Renderable
    {
        private readonly List<Renderable> _children;

        public VerticalLayout(
            IEnumerable<Renderable> children,
            Func<Margin> margin = null)
        {
            _children = children.ToList();
            MarginRenderable = new MarginRenderable(margin ?? (() => Margin.Zero), RenderChildren);
        }

        protected MarginRenderable MarginRenderable { get; }

        protected virtual void RenderChildren(RenderingContext context)
        {
            var width = context.Area.Width;
            var height = context.Area.Height;

            // calculate total requested height
            var hasSizeCount = 0;
            var totalRequestedHeight = _children
                .OfType<IHasSize>()
                .Sum(child =>
                {
                    var requestedHeight = child.Height();

                    if (requestedHeight.IsAuto)
                        return 0;

                    hasSizeCount++;
                    return requestedHeight;
                });

            if (totalRequestedHeight > height)
                throw new InvalidOperationException(
                    $"Total requested height {totalRequestedHeight} of children is larger than available space {height}");

            var autoHeight = (height - totalRequestedHeight) / (_children.Count - hasSizeCount);

            // do rendering
            var childX = 0;
            var childY = 0;
            foreach (var child in _children)
            {
                var childHeight = autoHeight;
                var childWidth = width;

                if (child is IHasSize hasSize)
                {
                    var requestedHeight = hasSize.Height();
                    if (!requestedHeight.IsAuto) childHeight = requestedHeight;

                    var requestedWidth = hasSize.Width();

                    if (!requestedWidth.IsAuto && requestedWidth < width)
                        childWidth = requestedWidth;
                }

                child.Render(context.Create(childX, childY, childWidth, childHeight));

                childY += childHeight;
            }
        }

        public override void Render(RenderingContext context)
        {
            MarginRenderable.Render(context);
        }
    }
}