using System.Device.Gpio;
using System.Diagnostics;
using Meadow.Hardware;

namespace Chibi.Hal.RpiZero2w;

public partial class GpioPin
{
    [DebuggerDisplay("Gpio(Output): {GpioPin.GPIO} = {State}")]
    public class OutputPort : IDigitalOutputPort
    {
        private bool _state;

        public OutputPort(GpioPin pin, bool initialValue)
        {
            GpioPin = pin;
            _state = initialValue;
            Controller.OpenPin(pin.GPIO, PinMode.Output, _state);
        }

        protected GpioController Controller => GpioPin._controller;

        protected GpioPin GpioPin { get; }

        public void Dispose()
        {
            if (Controller.IsPinOpen(GpioPin.GPIO))
                Controller.ClosePin(GpioPin.GPIO);
        }

        public IDigitalChannelInfo Channel => throw new NotImplementedException(nameof(Channel));

        public IPin Pin => GpioPin;

        public bool InitialState => throw new NotImplementedException(nameof(InitialState));

        public bool State
        {
            get => _state;
            set
            {
                _state = value;
                Controller.Write(GpioPin.GPIO, _state);
            }
        }
    }
}