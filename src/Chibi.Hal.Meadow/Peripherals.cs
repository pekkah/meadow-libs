using Meadow.Devices;
using Meadow.Gateways;
using Meadow.Hardware;
using Meadow.Units;

namespace Chibi.Hal.Meadow;

public static class Peripherals
{
    static Peripherals()
    {
        InternalDevice = new F7MicroV2();
        GPIO = new GPIOCHIP(InternalDevice);
    }

    public static GPIOCHIP GPIO { get; }

    public static IWiFiAdapter Wifi => InternalDevice.WiFiAdapter ??
                                       throw new InvalidOperationException("WiFiAdapter should not be null");

    public static IWatchdogController Watchdog => InternalDevice;

    private static F7MicroV2 InternalDevice { get; }

    public static II2cBus CreateI2C(Frequency frequency)
    {
        var i2c = GPIO.I2C;
        return InternalDevice.CreateI2cBus(i2c.SCL, i2c.SDA, frequency);
    }

    public static ISpiBus CreateSPI(SpiClockConfiguration config)
    {
        var spi = GPIO.SPI;
        return InternalDevice.CreateSpiBus(spi.SCLK, spi.MOSI, spi.MISO, config);
    }
}