using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;

namespace Verify.Wifi
{
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        private TaskCompletionSource<object?> _wifiConnectedSource;

        public MeadowApp()
        {
            _wifiConnectedSource = new TaskCompletionSource<object?>();
            Device.WiFiAdapter.WiFiConnected += (sender, args) =>
            {
                Console.WriteLine("Connected");
                _wifiConnectedSource.TrySetResult(null);
            };

            Device.WiFiAdapter.WiFiDisconnected += (sender, args) =>
            {
                Console.WriteLine("Disconnected");
                _wifiConnectedSource.SetCanceled();
            };
        }

        private async Task WaitConnected()
        {
            Console.WriteLine("Waiting connection");
            if (Device.WiFiAdapter.IsConnected)
                return;

            await _wifiConnectedSource.Task;
            _wifiConnectedSource = new TaskCompletionSource<object?>();
            Console.WriteLine("Connected");
        }

        public async Task RunAsync()
        {
            StartHeartbeat();

            OutputDeviceInfo();
            OutputMeadowOSInfo();

            OutputDeviceConfigurationInfo();

            var rgbPwmLed = new RgbPwmLed(
                Device,
                Device.Pins.OnboardLedRed,
                Device.Pins.OnboardLedGreen,
                Device.Pins.OnboardLedBlue);

            try
            {
                rgbPwmLed.SetColor(Color.Blue);
                await WaitConnected();
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
                    Console.WriteLine($"Request {++counter} {DateTime.Now}");
                    stopwatch.Restart();

                    // just in case we get stuck waiting for response
                    using var timeoutSource = new CancellationTokenSource();
                    timeoutSource.CancelAfter(TimeSpan.FromSeconds(90));

                    using var response = await client
                        .GetAsync("https://postman-echo.com/get?foo1=bar1&foo2=bar2", timeoutSource.Token)
                        .ConfigureAwait(false);

                    stopwatch.Stop();
                    Console.WriteLine(
                        $"Elapsed: {stopwatch.Elapsed.TotalSeconds}s, Status: {response.StatusCode}, GetTotalMemory: {GC.GetTotalMemory(false) / 1000}KB, {DateTime.Now}");
                    rgbPwmLed.Stop();
                    rgbPwmLed.SetColor(Color.Green);
                    await Task.Delay(TimeSpan.FromSeconds(5))
                        .ConfigureAwait(false);
                }
                catch (HttpRequestException x)
                {
                    rgbPwmLed.SetColor(Color.Yellow);
                    Console.WriteLine("Request failed {0}. Continuing after 1min delay.", x);
                    await Task.Delay(TimeSpan.FromMinutes(1))
                        .ConfigureAwait(false);

                    await WaitConnected();
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

        protected string FormatMacAddressString(byte[] address)
        {
            var result = string.Empty;

            for (var index = 0; index < address.Length; index++)
            {
                result += address[index].ToString("X2");
                if (index != address.Length - 1) result += ":";
            }

            return result;
        }

        private void OutputDeviceInfo()
        {
            Console.WriteLine($"Device name: {Device.Information.DeviceName}");
            Console.WriteLine($"Processor serial number: {Device.Information.ProcessorSerialNumber}");
            Console.WriteLine($"Processor ID: {Device.Information.ChipID}");
            Console.WriteLine($"Model: {Device.Information.Model}");
            Console.WriteLine($"Processor type: {Device.Information.ProcessorType}");
            Console.WriteLine($"Product: {Device.Information.Model}");
            Console.WriteLine($"Coprocessor type: {Device.Information.CoprocessorType}");
            Console.WriteLine($"Coprocessor firmware version: {Device.Information.CoprocessorOSVersion}");
        }

        private void OutputMeadowOSInfo()
        {
            Console.WriteLine($"OS version: {MeadowOS.SystemInformation.OSVersion}");
            Console.WriteLine($"Mono version: {MeadowOS.SystemInformation.MonoVersion}");
            Console.WriteLine($"Build date: {MeadowOS.SystemInformation.OSBuildDate}");
        }

        private void OutputDeviceConfigurationInfo()
        {
            try
            {
                Console.WriteLine($"Automatically connect to network: {Device.WiFiAdapter.AutomaticallyStartNetwork}");

                Console.WriteLine($"Automatically reconnect: {Device.WiFiAdapter.AutomaticallyReconnect}");

                Console.WriteLine($"Get time at startup: {Device.WiFiAdapter.GetNetworkTimeAtStartup}");
                //Console.WriteLine($"NTP Server: {Device.WiFiAdapter.NtpServer}");

                Console.WriteLine($"Default access point: {Device.WiFiAdapter.DefaultAcessPoint}");

                Console.WriteLine($"Maximum retry count: {Device.WiFiAdapter.MaximumRetryCount}");

                Console.WriteLine($"MAC address: {FormatMacAddressString(Device.WiFiAdapter.MacAddress)}");
                Console.WriteLine($"Soft AP MAC address: {FormatMacAddressString(Device.WiFiAdapter.ApMacAddress)}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        protected void StartHeartbeat()
        {
            Task.Run(async () => {
                while (true)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Beep...");
                    Console.ForegroundColor = color;
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
            });
        }

    }
}