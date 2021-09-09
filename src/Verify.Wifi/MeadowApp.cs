using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Gateway.WiFi;
using Meadow.Gateways;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Verify.Wifi
{
    public class Configuration
    {
        public string Ssid { get; set; }

        public string Password { get; set; }
    }

    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        private const string ConfigFilePath = "/meadow0/appSettings.Local.yml";

        public async Task RunAsync()
        {
            var rgbPwmLed = new RgbPwmLed(
                Device,
                Device.Pins.OnboardLedRed,
                Device.Pins.OnboardLedGreen,
                Device.Pins.OnboardLedBlue);


            if (!File.Exists(ConfigFilePath))
                throw new InvalidOperationException(
                    $"Create appSettings.Local.ym with following lines\r\nssid: \"YOUR_SSID\"\r\npassword: \"YOUR_PASSWORD\"");

            var configFile = await File.ReadAllTextAsync(ConfigFilePath);
            var input = new StringReader(configFile);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var configuration = deserializer.Deserialize<Configuration>(input);

            var networkName = configuration.Ssid;
            var networkPassword = configuration.Password;

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
                return;
            }


            var counter = 0;
            var stopwatch = new Stopwatch();


            Console.WriteLine($"Before request loop. GetTotalMemory: {GC.GetTotalMemory(false) / 1000}KB");

            // moving this inside the loop will cause the requests to pass faster but why??
            //using var client = new HttpClient();
            while (true)
            {
                try
                {
                    // comment out the above client to use new client per request
                    using var client = new HttpClient();
                    rgbPwmLed.SetColor(Color.LightYellow);
                    Console.WriteLine($"Request {++counter}");
                    stopwatch.Restart();

                    // normally on desktop I wouldn't use the dispose on the response
                    using var response = await client.GetAsync("https://postman-echo.com/get?foo1=bar1&foo2=bar2")
                        .ConfigureAwait(false);

                    stopwatch.Stop();
                    Console.WriteLine(
                        $"Elapsed: {stopwatch.Elapsed.TotalSeconds}s, Status: {response.StatusCode}, GetTotalMemory: {GC.GetTotalMemory(false) / 1000}KB");
                    rgbPwmLed.Stop();
                    rgbPwmLed.SetColor(Color.Green);
                    await Task.Delay(TimeSpan.FromSeconds(5))
                        .ConfigureAwait(false);
                }
                catch (HttpRequestException x)
                {
                    rgbPwmLed.SetColor(Color.Yellow);
                    Console.WriteLine("Request failed {0}. Continuing.", x);
                }
                catch (Exception ex)
                {
                    rgbPwmLed.SetColor(Color.Red);
                    Console.WriteLine(ex);
                    Console.WriteLine($"In exception. GetTotalMemory: {GC.GetTotalMemory(false) / 1000}KB");
                    break;
                }

                Console.WriteLine("Cycling...");
            }
        }
    }
}