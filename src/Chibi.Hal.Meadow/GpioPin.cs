using Meadow.Devices;
using Meadow.Hardware;

namespace Chibi.Hal.Meadow;

public class GpioPin : IPin
{
    private readonly F7MicroV2 _device;
    private readonly IPin _pin;

    public GpioPin(F7MicroV2 device, IPin pin)
    {
        _device = device;
        _pin = pin;
    }

    public IList<IChannelInfo>? SupportedChannels => _pin.SupportedChannels;

    public string Name => _pin.Name;

    public object Key => _pin.Key;

    public IDigitalOutputPort ToOutput(bool initialState = false)
    {
        return _device.CreateDigitalOutputPort(_pin, initialState);
    }

    public IDigitalInputPort ToInput(
        ResistorMode resistorMode,
        InterruptMode interruptMode,
        double debounceDuration = 0D,
        double glitchDuration = 0D)
    {
        return _device.CreateDigitalInputPort(_pin, interruptMode, resistorMode, debounceDuration, glitchDuration);
    }

    public IAnalogInputPort ToAnalogInput(int sampleCount = 5, int sampleIntervalMs = 40, float voltageReference = 3.3f)
    {
        return _device.CreateAnalogInputPort(_pin, sampleCount, sampleIntervalMs, voltageReference);
    }
}