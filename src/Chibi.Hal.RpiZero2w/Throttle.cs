using System.Diagnostics;

namespace Chibi.Hal.RpiZero2w;

public static class Throttle
{
    public static Action Wrap(Action action, TimeSpan time)
    {
        var lastTime = Stopwatch.GetTimestamp();

        return () =>
        {
            var timestamp = Stopwatch.GetTimestamp();
            if (timestamp - lastTime < time.Ticks)
                return;

            lastTime = timestamp;

            action();
        };
    }
}