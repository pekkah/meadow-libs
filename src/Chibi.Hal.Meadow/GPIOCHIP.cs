using Meadow.Devices;

namespace Chibi.Hal.Meadow;

public class GPIOCHIP
{
    public GPIOCHIP(F7MicroV2 internalDevice)
    {
        A00 = new GpioPin(internalDevice, internalDevice.Pins.A00);
        A01 = new GpioPin(internalDevice, internalDevice.Pins.A01);
        A02 = new GpioPin(internalDevice, internalDevice.Pins.A02);
        A03 = new GpioPin(internalDevice, internalDevice.Pins.A03);
        A04 = new GpioPin(internalDevice, internalDevice.Pins.A04);
        A05 = new GpioPin(internalDevice, internalDevice.Pins.A05);
        
        CIPO = new GpioPin(internalDevice, internalDevice.Pins.CIPO);
        COPI = new GpioPin(internalDevice, internalDevice.Pins.COPI);

        D00 = new GpioPin(internalDevice, internalDevice.Pins.D00);
        D01 = new GpioPin(internalDevice, internalDevice.Pins.D01);
        D02 = new GpioPin(internalDevice, internalDevice.Pins.D02);
        D03 = new GpioPin(internalDevice, internalDevice.Pins.D03);
        D04 = new GpioPin(internalDevice, internalDevice.Pins.D04);
        D05 = new GpioPin(internalDevice, internalDevice.Pins.D05);
        D06 = new GpioPin(internalDevice, internalDevice.Pins.D06);
        D07 = new GpioPin(internalDevice, internalDevice.Pins.D07);
        D08 = new GpioPin(internalDevice, internalDevice.Pins.D08);
        D09 = new GpioPin(internalDevice, internalDevice.Pins.D09);
        D10 = new GpioPin(internalDevice, internalDevice.Pins.D10);
        D11 = new GpioPin(internalDevice, internalDevice.Pins.D11);
        D12 = new GpioPin(internalDevice, internalDevice.Pins.D12);
        D13 = new GpioPin(internalDevice, internalDevice.Pins.D13);
        D14 = new GpioPin(internalDevice, internalDevice.Pins.D14);
        D15 = new GpioPin(internalDevice, internalDevice.Pins.D15);

        OnboardLedRed = new GpioPin(internalDevice, internalDevice.Pins.OnboardLedRed);
        OnboardLedGreen = new GpioPin(internalDevice, internalDevice.Pins.OnboardLedGreen);
        OnboardLedBlue = new GpioPin(internalDevice, internalDevice.Pins.OnboardLedBlue);

        SCK = new GpioPin(internalDevice, internalDevice.Pins.SCK);
    }

    public GpioPin A00 { get; }
    public GpioPin A01 { get; }
    public GpioPin A02 { get; }
    public GpioPin A03 { get; }
    public GpioPin A04 { get; }
    public GpioPin A05 { get; }
    public GpioPin CIPO { get; }
    public GpioPin COPI { get; }
    public GpioPin D00 { get; }
    public GpioPin D01 { get; }
    public GpioPin D02 { get; }
    public GpioPin D03 { get; }
    public GpioPin D04 { get; }
    public GpioPin D05 { get; }
    public GpioPin D06 { get; }
    public GpioPin D07 { get; }
    public GpioPin D08 { get; }
    public GpioPin D09 { get; }
    public GpioPin D10 { get; }
    public GpioPin D11 { get; }
    public GpioPin D12 { get; }
    public GpioPin D13 { get; }
    public GpioPin D14 { get; }
    public GpioPin D15 { get; }

    /// <summary>
    ///     I2C is a multi-drop bus, multiple devices can be connected to these same two pins. Each device has its own unique
    ///     I2C address.
    /// </summary>
    public (GpioPin SDA, GpioPin SCL) I2C => (D07, D08);

    public GpioPin OnboardLedBlue { get; }
    public GpioPin OnboardLedGreen { get; }
    public GpioPin OnboardLedRed { get; }

    public GpioPin SCK { get; }

    /// <summary>
    ///     Known as the four-wire serial bus, SPI lets you attach multiple compatible devices to a single set of pins by
    ///     assigning them different chip-select pins.
    ///     To talk to an SPI device, you assert its corresponding chip-select pin.
    /// </summary>
    public (GpioPin MOSI, GpioPin MISO, GpioPin SCLK) SPI => (COPI, CIPO, SCK);
}