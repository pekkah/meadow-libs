using System.Collections.Concurrent;
using System.Device.Gpio;
using System.Device.Spi;
using System.Diagnostics;
using Meadow.Hardware;
using Meadow.Units;

namespace Chibi.Hal.RpiZero2w;

[DebuggerDisplay("SpiBus: {BusId}")]
public class SpiBusAdapter : ISpiBus
{
    private readonly ConcurrentDictionary<IDigitalOutputPort, SpiDevice> _chipSelectToDevice = new();
    private SpiDevice? _noChipSelectDevice;

    public SpiBusAdapter(int busId, SpiClockConfiguration configuration)
    {
        BusId = busId;
        Configuration = configuration;
    }

    public int BusId { get; }

    public void Read(
        IDigitalOutputPort? chipSelect,
        Span<byte> readBuffer,
        ChipSelectMode csMode = ChipSelectMode.ActiveLow)
    {
        GetOrAddDevice(chipSelect, csMode).Read(readBuffer);
    }

    public void Write(
        IDigitalOutputPort? chipSelect,
        Span<byte> writeBuffer,
        ChipSelectMode csMode = ChipSelectMode.ActiveLow)
    {
        GetOrAddDevice(chipSelect, csMode).Write(writeBuffer);
    }

    public void Exchange(
        IDigitalOutputPort? chipSelect,
        Span<byte> writeBuffer,
        Span<byte> readBuffer,
        ChipSelectMode csMode = ChipSelectMode.ActiveLow)
    {
        GetOrAddDevice(chipSelect, csMode).TransferFullDuplex(writeBuffer, readBuffer);
    }

    public void SendData(IDigitalOutputPort chipSelect, params byte[] data)
    {
        Write(chipSelect, data);
    }

    public void SendData(IDigitalOutputPort chipSelect, IEnumerable<byte> data)
    {
        Write(chipSelect, data.ToArray());
    }

    public byte[] ReceiveData(IDigitalOutputPort chipSelect, int numberOfBytes)
    {
        Span<byte> buffer = new byte[numberOfBytes];
        Read(chipSelect, buffer);
        return buffer.ToArray();
    }

    public void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, params byte[] data)
    {
        Write(chipSelect, data, csMode);
    }

    public void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, IEnumerable<byte> data)
    {
        Write(chipSelect, data.ToArray(), csMode);
    }

    public byte[] ReceiveData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, int numberOfBytes)
    {
        Span<byte> buffer = new byte[numberOfBytes];
        Read(chipSelect, buffer, csMode);
        return buffer.ToArray();
    }

    public void ExchangeData(
        IDigitalOutputPort chipSelect,
        ChipSelectMode csMode,
        byte[] sendBuffer,
        byte[] receiveBuffer)
    {
        Exchange(chipSelect, sendBuffer, receiveBuffer, csMode);
    }

    public void ExchangeData(
        IDigitalOutputPort chipSelect,
        ChipSelectMode csMode,
        byte[] sendBuffer,
        byte[] receiveBuffer,
        int bytesToExchange)
    {
        Exchange(chipSelect, sendBuffer.AsSpan(0, bytesToExchange), receiveBuffer.AsSpan(0, bytesToExchange), csMode);
    }

    public Frequency[] SupportedSpeeds { get; }

    public SpiClockConfiguration Configuration { get; }

    protected SpiDevice GetOrAddDevice(IDigitalOutputPort? chipSelect, ChipSelectMode csMode = ChipSelectMode.ActiveLow)
    {
        if (_noChipSelectDevice is not null && chipSelect is null)
            return _noChipSelectDevice;

        if (chipSelect is null) return _noChipSelectDevice = SpiDevice.Create(NewSettings(null, csMode));

        return _chipSelectToDevice.GetOrAdd(chipSelect,
            newChipSelect => SpiDevice.Create(NewSettings(newChipSelect, csMode)));
    }

    protected SpiConnectionSettings NewSettings(IDigitalOutputPort? chipSelect, ChipSelectMode csMode)
    {
        return new SpiConnectionSettings(BusId, chipSelect != null ? (int)chipSelect.Pin.Key : -1)
        {
            ChipSelectLineActiveState = csMode == ChipSelectMode.ActiveLow ? PinValue.Low : PinValue.High,
            ClockFrequency = (int)Configuration.Speed.Hertz,
            DataBitLength = Configuration.BitsPerWord,
            Mode = Configuration.SpiMode switch
            {
                SpiClockConfiguration.Mode.Mode0 => SpiMode.Mode0,
                SpiClockConfiguration.Mode.Mode1 => SpiMode.Mode1,
                SpiClockConfiguration.Mode.Mode2 => SpiMode.Mode2,
                SpiClockConfiguration.Mode.Mode3 => SpiMode.Mode3,
                _ => throw new ArgumentOutOfRangeException()
            }
        };
    }
}