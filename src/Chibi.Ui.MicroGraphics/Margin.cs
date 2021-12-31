namespace Chibi.Ui.MicroGraphics
{
    public class Margin
    {
        public static readonly Margin Zero = new();

        public static readonly Margin Two = new(2, 2, 2, 2);

        public Margin(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Margin() : this(0, 0, 0, 0)
        {
        }

        public int Bottom { get; }

        public int Left { get; }
        public int Right { get; }
        public int Top { get; }
    }
}