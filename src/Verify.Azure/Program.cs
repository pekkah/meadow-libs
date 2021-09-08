using System.Threading;
using System.Threading.Tasks;

namespace Verify.Azure
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--exitOnDebug") return;

            await new MeadowApp()
                .RunAsync();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}