using System;
using System.IO;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Gateway.WiFi;
using Meadow.Gateways;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Verify.Azure
{
    public class Configuration
    {
        public string Ssid { get; set; }

        public string Password { get; set; }

        public string Hub { get; set; }
    }

    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        private Configuration _configuration;

        public async Task RunAsync()
        {
            var rgbPwmLed = new RgbPwmLed(
                Device,
                Device.Pins.OnboardLedRed,
                Device.Pins.OnboardLedGreen,
                Device.Pins.OnboardLedBlue);


            _configuration = await GetConfiguration();

            var networkName = _configuration.Ssid;
            var networkPassword = _configuration.Password;

            if (!await TryConnectWifi(rgbPwmLed, networkName, networkPassword))
                throw new InvalidOperationException($"Could not connect to {networkName}");

            Console.WriteLine($"GetTotalMemory: {GC.GetTotalMemory(false) / 1000}KB");

            using var client = await Connect();
        }

        private async Task<IDisposable> Connect()
        {
            
        }

        private static async Task<Configuration> GetConfiguration()
        {
            var configFile = await File.ReadAllTextAsync("/meadow0/appSettings.Local.yml");
            var input = new StringReader(configFile);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var configuration = deserializer.Deserialize<Configuration>(input);
            return configuration;
        }

        private static async Task<bool> TryConnectWifi(RgbPwmLed rgbPwmLed, string networkName, string networkPassword)
        {
            try
            {
                rgbPwmLed.SetColor(Color.Blue);
                Console.WriteLine("Initialize WiFi adapter...");
                await Device.InitWiFiAdapter()
                    .ConfigureAwait(false);

                if (Device.WiFiAdapter == null)
                    throw new InvalidOperationException("WIFI null");

                // this works with latest OS version (caused connection failure on earlier version)
                Device.WiFiAdapter.SetAntenna(AntennaType.External);

                Console.WriteLine($"Connecting to {networkName}...");
                ConnectionStatus connectionStatus;
                while ((connectionStatus = (await Device.WiFiAdapter.Connect(networkName, networkPassword)
                           .ConfigureAwait(false)).ConnectionStatus)
                       != ConnectionStatus.Success)
                {
                    rgbPwmLed.SetColor(Color.Red);
                    Console.WriteLine($"WiFi connect failed with {connectionStatus}");
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    MeadowOS.CurrentDevice.Reset();
                }

                Console.WriteLine("Network connected...");
            }
            catch (Exception ex)
            {
                rgbPwmLed.SetColor(Color.Red);
                Console.WriteLine(ex);
                return false;
            }

            return true;
        }
    }
}