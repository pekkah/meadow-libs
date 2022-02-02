using System.Device.Gpio;
using System.Diagnostics;
using Meadow;
using Meadow.Hardware;

namespace Chibi.Hal.RpiZero2w;

public partial class GpioPin
{
    [DebuggerDisplay("Gpio(Input): {GpioPin.GPIO} = {State}")]
    public class InputPort : IDigitalInputPort
    {
        public InputPort(GpioPin pin, ResistorMode resistor, InterruptMode interruptMode)
        {
            _lastPinValueChange = Stopwatch.GetTimestamp();
            GpioPin = pin;
            Controller.OpenPin(pin.GPIO, GetPinInputMode(resistor));

            Resistor = resistor;
            InterruptMode = interruptMode;
            Controller.RegisterCallbackForPinValueChangedEvent(
                GpioPin.GPIO,
                GetPinEventMode(interruptMode),
                PinValueChanged);
        }

        protected GpioController Controller => GpioPin._controller;

        protected GpioPin GpioPin { get; }
        private DateTime LastEventTime { get; set; } = DateTime.MinValue;

        public void Dispose()
        {
            if (Controller.IsPinOpen(GpioPin.GPIO))
                Controller.ClosePin(GpioPin.GPIO);
        }

        public InterruptMode InterruptMode { get; }

        public double DebounceDuration { get; set; }

        private long DebounceTicks => (long)DebounceDuration * TimeSpan.TicksPerMillisecond;

        /// <inheritdoc />
        public double GlitchDuration { get; set; }

        public event EventHandler<DigitalPortResult>? Changed;

        public IDigitalChannelInfo Channel => throw new NotImplementedException(nameof(Channel));

        public IPin Pin => GpioPin;

        public IDisposable Subscribe(IObserver<IChangeResult<DigitalState>> observer)
        {
            throw new NotImplementedException();
        }

        public bool State => (bool)Controller.Read(GpioPin.GPIO);

        public ResistorMode Resistor
        {
            get
            {
                return Controller.GetPinMode(GpioPin.GPIO) switch
                {
                    PinMode.Input => ResistorMode.Disabled,
                    PinMode.Output => throw new InvalidOperationException("Input port"),
                    PinMode.InputPullDown => ResistorMode.InternalPullDown,
                    PinMode.InputPullUp => ResistorMode.InternalPullUp,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            set => Controller.SetPinMode(GpioPin.GPIO, GetPinInputMode(value));
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

        private long _lastPinValueChange;

        private void PinValueChanged(object sender, PinValueChangedEventArgs args)
        {
            // throttle
            var timestamp = Stopwatch.GetTimestamp();
            if (timestamp - _lastPinValueChange <= DebounceTicks)
                return;

            _lastPinValueChange = timestamp;

            if (Changed != null)
            {
                var lastEventTime = LastEventTime;
                LastEventTime = DateTime.Now;
                var oldState = lastEventTime == DateTime.MinValue
                    ? new DigitalState?()
                    : new DigitalState(!State, lastEventTime);
                
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
}