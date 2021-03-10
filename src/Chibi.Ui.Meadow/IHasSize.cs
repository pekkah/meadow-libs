using System;

namespace Chibi.Ui.Meadow
{
    public interface IHasSize
    {
        Func<Length> Width { get; }

        Func<Length> Height { get; }
    }
}