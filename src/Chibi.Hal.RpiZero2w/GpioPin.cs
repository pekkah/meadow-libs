using System.Device.Gpio;
using Meadow;
using Meadow.Hardware;

namespace Chibi.Hal.RpiZero2w;

public class GpioPin : IPin
{
    private readonly GpioController _controller;

    public GpioPin(GpioController controller, string name, object key, IList<IChannelInfo>? supportedChannels = null)
    {
        _controller = controller;
        Name = name;
        Key = key;
        SupportedChannels = supportedChannels;
        LineNumber = (int)key;
    }

    public bool IsOpen => _controller.IsPinOpen(LineNumber);

    public int LineNumber { get; }

    public IList<IChannelInfo>? SupportedChannels { get; }

    public string Name { get; }

    public object Key { get; }

    public override string ToString()
    {
        return Name;
    }

    public IDigitalOutputPort AsOutput(bool initialValue = false)
    {
        if (IsOpen)
            throw new InvalidOperationException($"Pin {LineNumber} is open");

        return new OutputPort(this, initialValue);
    }

    public IDigitalInputPort AsInput(ResistorMode resistorMode, InterruptMode interruptMode)
    {
        if (IsOpen)
            throw new InvalidOperationException($"Pin {LineNumber} is open");

        return new InputPort(this, resistorMode, interruptMode);
    }

    public class InputPort : IDigitalInputPort
    {
        private DateTime LastEventTime { get; set; } = DateTime.MinValue;

        public InputPort(GpioPin pin, ResistorMode resistor, InterruptMode interruptMode)
        {
            GpioPin = pin;
            Controller.OpenPin(pin.LineNumber, GetPinInputMode(resistor));

            Resistor = resistor;
            InterruptMode = interruptMode;
            Controller.RegisterCallbackForPinValueChangedEvent(
                GpioPin.LineNumber, 
                GetPinEventMode(interruptMode),
                PinValueChanged);
        }

        protected GpioController Controller => GpioPin._controller;

        protected GpioPin GpioPin { get; }

        public void Dispose()
        {
            if (Controller.IsPinOpen(GpioPin.LineNumber))
                Controller.ClosePin(GpioPin.LineNumber);
        }

        public InterruptMode InterruptMode { get; }

        public double DebounceDuration { get; set; }

        public double GlitchDuration { get; set; }

        public event EventHandler<DigitalPortResult>? Changed;

        public IDigitalChannelInfo Channel => throw new NotImplementedException(nameof(Channel));

        public IPin Pin => GpioPin;

        public IDisposable Subscribe(IObserver<IChangeResult<DigitalState>> observer)
        {
            throw new NotImplementedException();
        }

        public bool State => (bool)Controller.Read(GpioPin.LineNumber);

        public ResistorMode Resistor
        {
            get
            {
                return Controller.GetPinMode(GpioPin.LineNumber) switch
                {
                    PinMode.Input => ResistorMode.Disabled,
                    PinMode.Output => throw new InvalidOperationException("Input port"),
                    PinMode.InputPullDown => ResistorMode.InternalPullDown,
                    PinMode.InputPullUp => ResistorMode.InternalPullUp,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            set => Controller.SetPinMode(GpioPin.LineNumber, GetPinInputMode(value));
        }

        private PinEventTypes GetPinEventMode(InterruptMode interruptMode)
        {
            return interruptMode switch
            {
                InterruptMode.None => PinEventTypes.None,
                InterruptMode.EdgeFalling => PinEventTypes.Falling,
                InterruptMode.EdgeRising => PinEventTypes.Rising,
                InterruptMode.EdgeBoth => PinEventTypes.Rising | PinEventTypes.Falling,
                _ => throw new ArgumentOutOfRangeException(nameof(interruptMode), interruptMode, null)
            };
        }

        private void PinValueChanged(object sender, PinValueChangedEventArgs args)
        {
            if (Changed != null)
            {
                DateTime lastEventTime = LastEventTime;
                LastEventTime = DateTime.Now;
                DigitalState? oldState = lastEventTime == DateTime.MinValue ? new DigitalState?() : new DigitalState(!State, lastEventTime);
                Changed(this, new DigitalPortResult(new DigitalState(State, LastEventTime), oldState));
            }
        }

        private PinMode GetPinInputMode(ResistorMode resistor)
        {
            return resistor switch
            {
                ResistorMode.Disabled => PinMode.Input,
                ResistorMode.InternalPullDown => PinMode.InputPullDown,
                ResistorMode.InternalPullUp => PinMode.InputPullUp,
                ResistorMode.ExternalPullDown => PinMode.Input,
                ResistorMode.ExternalPullUp => PinMode.Input,
                _ => throw new ArgumentOutOfRangeException(nameof(resistor), resistor, null)
            };
        }
    }

    public class OutputPort : IDigitalOutputPort
    {
        private bool _state;

        public OutputPort(GpioPin pin, bool initialValue)
        {
            GpioPin = pin;
            _state = initialValue;
            Controller.OpenPin(pin.LineNumber, PinMode.Output, _state);
        }

        protected GpioController Controller => GpioPin._controller;

        protected GpioPin GpioPin { get; }

        public void Dispose()
        {
            if (Controller.IsPinOpen(GpioPin.LineNumber))
                Controller.ClosePin(GpioPin.LineNumber);
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
                Controller.Write(GpioPin.LineNumber, _state);
            }
        }
    }
}