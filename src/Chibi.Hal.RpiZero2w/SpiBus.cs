using Meadow.Hardware;
using Meadow.Units;

namespace Chibi.Hal.RpiZero2w;

public class SpiBus : ISpiBus
{
    public void Read(IDigitalOutputPort? chipSelect, Span<byte> readBuffer, ChipSelectMode csMode = ChipSelectMode.ActiveLow)
    {
        throw new NotImplementedException();
    }

    public void Write(IDigitalOutputPort? chipSelect, Span<byte> writeBuffer, ChipSelectMode csMode = ChipSelectMode.ActiveLow)
    {
        throw new NotImplementedException();
    }

    public void Exchange(IDigitalOutputPort? chipSelect, Span<byte> writeBuffer, Span<byte> readBuffer, ChipSelectMode csMode = ChipSelectMode.ActiveLow)
    {
        throw new NotImplementedException();
    }

    public void SendData(IDigitalOutputPort chipSelect, params byte[] data)
    {
        throw new NotImplementedException();
    }

    public void SendData(IDigitalOutputPort chipSelect, IEnumerable<byte> data)
    {
        throw new NotImplementedException();
    }

    public byte[] ReceiveData(IDigitalOutputPort chipSelect, int numberOfBytes)
    {
        throw new NotImplementedException();
    }

    public void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, params byte[] data)
    {
        throw new NotImplementedException();
    }

    public void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, IEnumerable<byte> data)
    {
        throw new NotImplementedException();
    }

    public byte[] ReceiveData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, int numberOfBytes)
    {
        throw new NotImplementedException();
    }

    public void ExchangeData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, byte[] sendBuffer, byte[] receiveBuffer)
    {
        throw new NotImplementedException();
    }

    public void ExchangeData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, byte[] sendBuffer, byte[] receiveBuffer,
        int bytesToExchange)
    {
        throw new NotImplementedException();
    }

    public Frequency[] SupportedSpeeds { get; }

    public SpiClockConfiguration Configuration { get; }
}