using System.Threading.Tasks;

namespace Verify.Wifi
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--exitOnDebug") return;

            await new MeadowApp()
                .RunAsync();
        }
    }
}