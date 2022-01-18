namespace Chibi.Ui.SourceGenerators
{
    public class AtlasPartDefinition
    {
        public int BytesLength { get; set; }

        public string FileName { get; set; } = null!;

        public int Height { get; set; }
        public int Width { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
    }
}