using System.Threading.Tasks;

namespace Chibi.Verify.Https.MeadowF7v2
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