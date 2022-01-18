using VerifyXunit;
using Xunit;

namespace Chibi.Ui.SourceGenerators.Tests;

[UsesVerify]
public class ImageAtlasSnapshotTests
{
    [Fact]
    public Task Generate_From_Images()
    {
        // The source code to test
        var source = @"
using Chibi.Ui.Micrographics;

namespace Chibi.Ui.Tests;

[ImageAtlas(RelativeSourcePath = ""Icons"", BufferType = ""Buffer1bpp"")]
public partial class Icons 
{
    
}";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper<ImageAtlasGenerator>.Verify(source);
    }
}