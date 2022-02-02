using System.Collections.Concurrent;
using System.Device.I2c;
using System.Diagnostics;
using Meadow.Hardware;
using Meadow.Units;

namespace Chibi.Hal.RpiZero2w;

[DebuggerDisplay("I2cBus: {BusId}")]
public class I2cBusAdapter : II2cBus
{
    public int BusId { get; }
    private readonly ConcurrentDictionary<byte, I2cDevice> _addressToDevice = new();
    private readonly I2cBus _internalBus;

    public I2cBusAdapter(int busId)
    {
        BusId = busId;
        _internalBus = I2cBus.Create(busId);
    }

    public void Dispose()
    {
        _internalBus.Dispose();
    }

    public void Read(byte peripheralAddress, Span<byte> readBuffer)
    {
        GetOrAddDevice(peripheralAddress).Read(readBuffer);
    }

    public void Write(byte peripheralAddress, Span<byte> writeBuffer)
    {
        GetOrAddDevice(peripheralAddress).Write(writeBuffer);
    }

    public void Exchange(byte peripheralAddress, Span<byte> writeBuffer, Span<byte> readBuffer)
    {
        GetOrAddDevice(peripheralAddress).WriteRead(writeBuffer, readBuffer);
    }

    public void WriteData(byte peripheralAddress, params byte[] data)
    {
        Write(peripheralAddress, data);
    }

    public void WriteData(byte peripheralAddress, byte[] data, int length)
    {
        Write(peripheralAddress, data.AsSpan(0, length));
    }

    public void WriteData(byte peripheralAddress, IEnumerable<byte> data)
    {
        Write(peripheralAddress, data.ToArray());
    }

    public byte[] WriteReadData(byte peripheralAddress, int byteCountToRead, params byte[] dataToWrite)
    {
        Write(peripheralAddress, dataToWrite);

        var readBuffer = new byte[byteCountToRead];
        Read(peripheralAddress, readBuffer);

        return readBuffer;
    }

    public byte[] ReadData(byte peripheralAddress, int numberOfBytes)
    {
        var readBuffer = new byte[numberOfBytes];
        Read(peripheralAddress, readBuffer);

        return readBuffer;
    }

    public Frequency Frequency { get; set; }

    protected I2cDevice GetOrAddDevice(byte address)
    {
        return _addressToDevice.GetOrAdd(address, newAddress => _internalBus.CreateDevice(newAddress));
    }
}