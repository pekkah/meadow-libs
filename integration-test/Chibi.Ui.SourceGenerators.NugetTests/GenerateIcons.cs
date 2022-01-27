using Chibi.Ui.MicroGraphics;

namespace Chibi.Ui.SourceGenerators.NugetTests;

[MicroGraphicsBuffers(RelativeSourcePath = "Icons", BufferType = "BufferRgb888")]
public partial class GenerateIcons 
{
    [Fact]
    public void Test()
    {
        Assert.NotNull(Play16x16);
        Assert.NotNull(Play32x32);
    }
}