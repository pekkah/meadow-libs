using System.IO;
using System.Text;
using Meadow.Hardware;

namespace Chibi.Infrastructure
{
    public class SerialMessageTextWriter : TextWriter
    {
        private readonly ISerialMessagePort _serial;

        public SerialMessageTextWriter(ISerialMessagePort serial)
        {
            _serial = serial;
        }

        public override Encoding Encoding { get; } = Encoding.UTF8;


        public override void Write(char value)
        {
            if (_serial.IsOpen)
                _serial.Write(Encoding.UTF8.GetBytes(new[] {value}));
        }
    }
}