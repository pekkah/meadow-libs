using System.Runtime.InteropServices;
using Meadow.Hardware;
using Meadow.Units;

namespace Chibi.Hal.RpiZero2w;

public class I2CBus : II2cBus
{
    private const string DefaultDevicePath = "/dev/i2c";
    private static readonly object InitializationLock = new();

    public unsafe I2CBus(int busId)
    {
        var deviceFileName = $"{DefaultDevicePath}-{busId}";
        lock (InitializationLock)
        {
            var busFileDescriptor = Interop.open(deviceFileName, FileOpenFlags.O_RDWR);

            if (busFileDescriptor < 0)
                throw new IOException(
                    $"Error {Marshal.GetLastWin32Error()}. Can not open I2C device file '{deviceFileName}'.");

            I2cFunctionalityFlags functionalityFlags;
            var result = Interop.ioctl(busFileDescriptor, (uint)I2cSettings.I2C_FUNCS, new IntPtr(&functionalityFlags));
            if (result < 0) functionalityFlags = 0;

            if ((functionalityFlags & I2cFunctionalityFlags.I2C_FUNC_I2C) != 0)
            {
                BusId = busId;
                BusFileDescriptor = busFileDescriptor;
            }
            else
            {
                throw new InvalidOperationException("Could not initialize bus");
            }
        }
    }

    public int BusId { get; }
    protected int BusFileDescriptor { get; private set; }

    public void Dispose()
    {
        if (BusFileDescriptor > 0)
        {
            Interop.close(BusFileDescriptor);
            BusFileDescriptor = -1;
        }
    }

    public unsafe void Read(byte peripheralAddress, Span<byte> readBuffer)
    {
        if (readBuffer.Length == 0)
            throw new ArgumentException($"{nameof(readBuffer)} cannot be empty.", nameof(readBuffer));

        if (readBuffer.Length > ushort.MaxValue)
            throw new ArgumentException($"{nameof(readBuffer)} length is too long.", nameof(readBuffer));

        fixed (byte* readBufferPointer = readBuffer)
        {
            WriteReadCore(peripheralAddress, null, readBufferPointer, 0, (ushort)readBuffer.Length);
        }
    }

    public unsafe void Write(byte peripheralAddress, Span<byte> writeBuffer)
    {
        if (writeBuffer.Length > ushort.MaxValue)
            throw new ArgumentException($"{nameof(writeBuffer)} length is too long.", nameof(writeBuffer));

        fixed (byte* writeBufferPointer = writeBuffer)
        {
            WriteReadCore(peripheralAddress, writeBufferPointer, null, (ushort)writeBuffer.Length, 0);
        }
    }

    public unsafe void Exchange(byte peripheralAddress, Span<byte> writeBuffer, Span<byte> readBuffer)
    {
        if (readBuffer.Length == 0)
            throw new ArgumentException($"{nameof(readBuffer)} cannot be empty.", nameof(readBuffer));

        if (writeBuffer.Length > ushort.MaxValue)
            throw new ArgumentException($"{nameof(writeBuffer)} length is too long.", nameof(writeBuffer));

        if (readBuffer.Length > ushort.MaxValue)
            throw new ArgumentException($"{nameof(readBuffer)} length is too long.", nameof(readBuffer));

        fixed (byte* writeBufferPointer = writeBuffer)
        {
            fixed (byte* readBufferPointer = readBuffer)
            {
                WriteReadCore(peripheralAddress, writeBufferPointer, readBufferPointer, (ushort)writeBuffer.Length,
                    (ushort)readBuffer.Length);
            }
        }
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

    protected virtual unsafe void WriteReadCore(ushort deviceAddress, byte* writeBuffer, byte* readBuffer,
        ushort writeBufferLength, ushort readBufferLength)
    {
        // Allocating space for 2 messages in case we want to read and write on the same call.
        var messagesPtr = stackalloc i2c_msg[2];
        var messageCount = 0;

        if (writeBuffer != null)
            messagesPtr[messageCount++] = new i2c_msg
            {
                flags = I2cMessageFlags.I2C_M_WR,
                addr = deviceAddress,
                len = writeBufferLength,
                buf = writeBuffer
            };

        if (readBuffer != null)
            messagesPtr[messageCount++] = new i2c_msg
            {
                flags = I2cMessageFlags.I2C_M_RD,
                addr = deviceAddress,
                len = readBufferLength,
                buf = readBuffer
            };

        var msgset = new i2c_rdwr_ioctl_data
        {
            msgs = messagesPtr,
            nmsgs = (uint)messageCount
        };

        var result = Interop.ioctl(BusFileDescriptor, (uint)I2cSettings.I2C_RDWR, new IntPtr(&msgset));
        if (result < 0) throw new IOException($"Error {Marshal.GetLastWin32Error()} performing I2C data transfer.");
    }
}