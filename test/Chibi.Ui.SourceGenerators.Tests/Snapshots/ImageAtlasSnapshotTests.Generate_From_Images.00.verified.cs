//HintName: ImageAtlasAttribute.cs
namespace Chibi.Ui.Micrographics
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ImageAtlasAttribute : System.Attribute
    {
        public string RelativeSourcePath { get; set; }

        public string BufferType { get; set; }
    }
}