using System;

namespace Chibi.Ui.MicroGraphics
{
    public interface IHasSize
    {
        Func<Length> Height { get; }
        Func<Length> Width { get; }
    }
}