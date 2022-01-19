//HintName: MicroGraphicsBuffersAttribute.cs
namespace Chibi.Ui.MicroGraphics
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class MicroGraphicsBuffersAttribute : System.Attribute
    {
        public string RelativeSourcePath { get; set; }

        public string BufferType { get; set; }
    }
}