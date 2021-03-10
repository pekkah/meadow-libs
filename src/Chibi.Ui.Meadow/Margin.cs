namespace Chibi.Ui.Meadow
{
    public class Margin
    {
        public static readonly Margin Zero = new Margin();

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

        public int Left { get; }
        public int Top { get; }
        public int Right { get; }
        public int Bottom { get; }
    }
}