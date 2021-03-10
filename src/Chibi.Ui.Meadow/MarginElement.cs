using System;

namespace Chibi.Ui.Meadow
{
    public class MarginRenderable : Renderable
    {
        private readonly Action<RenderingContext> _renderChild;

        public MarginRenderable(Func<Margin> margin, Action<RenderingContext> renderChild)
        {
            _renderChild = renderChild;
            Margin = margin;
        }

        public Func<Margin> Margin { get; }

        public override void Render(RenderingContext context)
        {
            var margin = Margin();
            var x = margin.Left;
            var y = margin.Top;
            var width = context.Area.Width - (margin.Left + margin.Right);
            var height = context.Area.Height - (margin.Top + margin.Bottom);

            RenderChild(context.Create(x, y, width, height));
        }

        protected virtual void RenderChild(RenderingContext context)
        {
            _renderChild(context);
        }
    }
}