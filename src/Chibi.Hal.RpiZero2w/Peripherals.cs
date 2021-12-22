using Meadow.Hardware;

namespace Chibi.Hal.RpiZero2w;

public static class Peripherals
{
    /// <summary>
    ///     See pinout at https://pinout.xyz/
    /// </summary>
    public static GPIOCHIP0 GPIOCHIP0 { get; } = new();

    public static II2cBus CreateI2C1()
    {
        var i2c1 = GPIOCHIP0.I2C1;
        //todo: lock pins
        return new I2CBus(1);
    }
    
    public static ISpiBus CreateSPI0()
    {
        var spi0 = GPIOCHIP0.SPI0;

        return new SpiBus();
    }
}