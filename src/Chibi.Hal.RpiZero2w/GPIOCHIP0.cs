using System.Device.Gpio;
using System.Device.Gpio.Drivers;

namespace Chibi.Hal.RpiZero2w;

/// <summary>
///     See pinout at https://pinout.xyz/
/// </summary>
public class GPIOCHIP0 : IDisposable
{
    private readonly GpioController _controller;

    public GPIOCHIP0()
    {
        _controller = new GpioController(PinNumberingScheme.Logical, new LibGpiodDriver());
        GPIO02 = new GpioPin(_controller, nameof(GPIO02), 2);
        GPIO03 = new GpioPin(_controller, nameof(GPIO03), 3);
        GPIO04 = new GpioPin(_controller, nameof(GPIO04), 4);
        GPIO17 = new GpioPin(_controller, nameof(GPIO17), 17);
        GPIO27 = new GpioPin(_controller, nameof(GPIO27), 27);
        GPIO22 = new GpioPin(_controller, nameof(GPIO22), 22);
        GPIO10 = new GpioPin(_controller, nameof(GPIO10), 10);
        GPIO09 = new GpioPin(_controller, nameof(GPIO09), 9);
        GPIO11 = new GpioPin(_controller, nameof(GPIO11), 11);
        GPIO00 = new GpioPin(_controller, nameof(GPIO00), 0);
        GPIO05 = new GpioPin(_controller, nameof(GPIO05), 5);
        GPIO06 = new GpioPin(_controller, nameof(GPIO06), 6);
        GPIO13 = new GpioPin(_controller, nameof(GPIO13), 13);
        GPIO19 = new GpioPin(_controller, nameof(GPIO19), 19);
        GPIO26 = new GpioPin(_controller, nameof(GPIO26), 26);

        GPIO14 = new GpioPin(_controller, nameof(GPIO14), 14);
        GPIO15 = new GpioPin(_controller, nameof(GPIO15), 15);
        GPIO18 = new GpioPin(_controller, nameof(GPIO18), 18);
        GPIO23 = new GpioPin(_controller, nameof(GPIO23), 23);
        GPIO24 = new GpioPin(_controller, nameof(GPIO24), 24);
        GPIO25 = new GpioPin(_controller, nameof(GPIO25), 25);
        GPIO08 = new GpioPin(_controller, nameof(GPIO08), 8);
        GPIO07 = new GpioPin(_controller, nameof(GPIO07), 7);
        GPIO01 = new GpioPin(_controller, nameof(GPIO01), 1);
        GPIO12 = new GpioPin(_controller, nameof(GPIO12), 12);
        GPIO16 = new GpioPin(_controller, nameof(GPIO16), 16);
        GPIO20 = new GpioPin(_controller, nameof(GPIO20), 20);
        GPIO21 = new GpioPin(_controller, nameof(GPIO21), 21);
    }

    public GpioPin GPIO00 { get; set; }

    public GpioPin GPIO01 { get; set; }

    public GpioPin GPIO02 { get; }

    public GpioPin GPIO03 { get; }

    public GpioPin GPIO04 { get; set; }

    public GpioPin GPIO05 { get; set; }

    public GpioPin GPIO06 { get; set; }

    public GpioPin GPIO07 { get; set; }

    public GpioPin GPIO08 { get; set; }

    public GpioPin GPIO09 { get; set; }

    public GpioPin GPIO10 { get; set; }

    public GpioPin GPIO11 { get; set; }

    public GpioPin GPIO12 { get; set; }

    public GpioPin GPIO13 { get; set; }

    public GpioPin GPIO14 { get; set; }

    public GpioPin GPIO15 { get; set; }

    public GpioPin GPIO16 { get; set; }

    public GpioPin GPIO17 { get; set; }

    public GpioPin GPIO18 { get; set; }

    public GpioPin GPIO19 { get; set; }

    public GpioPin GPIO20 { get; set; }

    public GpioPin GPIO21 { get; set; }

    public GpioPin GPIO22 { get; set; }

    public GpioPin GPIO23 { get; set; }

    public GpioPin GPIO24 { get; set; }

    public GpioPin GPIO25 { get; set; }

    public GpioPin GPIO26 { get; set; }

    public GpioPin GPIO27 { get; set; }

    /// <summary>
    ///     I2C0 - can be used as an alternate I2C bus, but are typically used by the system to read the HAT EEPROM.
    /// </summary>
    public (GpioPin SDA, GpioPin SCL) I2C0 => (GPIO00, GPIO01);

    /// <summary>
    ///     GPIO 2 and GPIO 3 - the Raspberry Pi's I2C1 pins - allow for two-wire communication with a variety of external
    ///     sensors and devices.
    ///     The I2C pins include a fixed 1.8 kΩ pull-up resistor to 3.3v. They are not suitable for use as general purpose IO
    ///     where a pull-up might interfere.
    ///     I2C is a multi-drop bus, multiple devices can be connected to these same two pins. Each device has its own unique
    ///     I2C address.
    /// </summary>
    public (GpioPin SDA, GpioPin SCL) I2C1 => (GPIO02, GPIO03);

    /// <summary>
    ///     Raspberry Pi. There are 4 PWM pins on the Raspberry Pi, but each pair of the 4 pins is sharing one PWM resource.
    ///     GPIO12 and GPIO 18 are sharing one PWM channel while GPIO 13 and GPIO 19 are sharing on the other one. This means
    ///     that there are only 2 unique/controllable PWM channels on the pi.
    /// </summary>
    public (GpioPin PWM0_0, GpioPin PWM0_1) PWM0 => (GPIO18, GPIO12);

    /// <summary>
    ///     Raspberry Pi. There are 4 PWM pins on the Raspberry Pi, but each pair of the 4 pins is sharing one PWM resource.
    ///     GPIO12 and GPIO 18 are sharing one PWM channel while GPIO 13 and GPIO 19 are sharing on the other one. This means
    ///     that there are only 2 unique/controllable PWM channels on the pi.
    /// </summary>
    public (GpioPin PWM1_0, GpioPin PWM1_1) PWM1 => (GPIO13, GPIO19);

    /// <summary>
    ///     Known as the four-wire serial bus, SPI lets you attach multiple compatible devices to a single set of pins by
    ///     assigning them different chip-select pins.
    ///     To talk to an SPI device, you assert its corresponding chip-select pin.
    ///     By default the Pi allows you to use SPI0 with chip select pins on CE0 on GPIO 8 and CE1 on GPIO 7.
    /// </summary>
    public (GpioPin MOSI, GpioPin MISO, GpioPin SCLK, GpioPin CE0, GpioPin CE1) SPI0 =>
        (GPIO10, GPIO09, GPIO11, GPIO08, GPIO07);

    /// <summary>
    ///     You can enable SPI1 with a dtoverlay configured in "/boot/config.txt", for example:
    ///     ```
    ///     dtoverlay=spi1-3cs
    ///     ```
    /// </summary>
    public (GpioPin MOSI, GpioPin MISO, GpioPin SCLK, GpioPin CE0, GpioPin CE1, GpioPin CE02) SPI1 =>
        (GPIO20, GPIO19, GPIO21, GPIO18, GPIO17, GPIO16);

    public void Dispose()
    {
        _controller.Dispose();
    }
}