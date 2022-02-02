using System.Device.Gpio;
using System.Diagnostics;
using Meadow.Hardware;

namespace Chibi.Hal.RpiZero2w;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public partial class GpioPin : IPin
{
    private readonly GpioController _controller;

    public GpioPin(GpioController controller, string name, object key, IList<IChannelInfo>? supportedChannels = null)
    {
        _controller = controller;
        Name = name;
        Key = key;
        SupportedChannels = supportedChannels;
        GPIO = (int)key;
    }

    public int GPIO { get; }

    public bool IsOpen => _controller.IsPinOpen(GPIO);

    private string DebuggerDisplay => $"Gpio: {GPIO}, IsOpen: {IsOpen}";

    public IList<IChannelInfo>? SupportedChannels { get; }

    public string Name { get; }

    public object Key { get; }

    public override string ToString()
    {
        return Name;
    }

    public IDigitalOutputPort ToOutput(bool initialValue = false)
    {
        if (IsOpen)
            throw new InvalidOperationException($"Pin {GPIO} is open");

        return new OutputPort(this, initialValue);
    }

    public IDigitalInputPort ToInput(ResistorMode resistorMode, InterruptMode interruptMode, long debounceMs = 0)
    {
        if (IsOpen)
            throw new InvalidOperationException($"Pin {GPIO} is open");

        return new InputPort(this, resistorMode, interruptMode)
        {
            DebounceDuration = debounceMs
        };
    }
}