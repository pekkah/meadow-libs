namespace Chibi.Ui.SourceGenerators.Tests;

[UsesVerify]
public class MicroGraphicsBuffersGeneratorTests
{
    [Fact]
    public Task Generate_From_Images()
    {
        // The source code to test
        var source = @"
using Chibi.Ui.MicroGraphics;

namespace Chibi.Ui.Tests;

[MicroGraphicsBuffers(RelativeSourcePath = ""Icons"", BufferType = ""Buffer1bpp"")]
public partial class Icons 
{
    
}";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper<MicroGraphicsBuffersGenerator>.Verify(source);
    }
}