using Meadow.Gateways;

namespace Chibi.Hal.Meadow;

public static class WiFiAdapterExtensions
{
    private static readonly Task<bool> True = Task.FromResult(true);

    public static Task<bool> Connect(this IWiFiAdapter adapter)
    {
        if (adapter.IsConnected)
            return True;

        var tsc = new TaskCompletionSource<bool>();

        Task.Factory.StartNew(async () =>
        {
            while (!adapter.IsConnected) await Task.Delay(25);

            tsc.SetResult(true);
        });

        return tsc.Task;
    }
}