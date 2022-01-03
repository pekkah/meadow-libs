using System;
using System.IO;
using System.Runtime.InteropServices;
using Meadow.Foundation.Graphics.Buffers;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Chibi.Ui.Simple.Screens;

public static class ImageLoader
{
    private static readonly ILogger Logger = Log.ForContext(typeof(ImageLoader));

    public static bool HalfSize { get; set; } = false;

    public static IDisplayBuffer LoadBgr(string path)
    {
        try
        {
            var currentPath = AppContext.BaseDirectory;
            var filePath = Path.Combine(currentPath, path);
            var filename = Path.GetFileName(filePath);

            if (HalfSize)
            {
                Logger.Information("Use half-size images");
                filePath = filePath.Replace(filename, $"16/{filename}");
            }
            else
            {
                filePath = filePath.Replace(filename, $"32/{filename}");
            }

            using var image = Image.Load(File.ReadAllBytes(filePath));
            using var bgr = image.CloneAs<Bgr24>();
            if (bgr.TryGetSinglePixelSpan(out var bgrSpan))
            {
                var span = MemoryMarshal.AsBytes(bgrSpan);
                var bytes = span.ToArray();

                var buffer = new BufferRgb888(bgr.Width, bgr.Height, bytes);
                return buffer;
            }

            throw new InvalidOperationException($"Could not read '{path}'");
        }
        catch (Exception x)
        {
            Logger.Error(x, "Failed to load '{path}'", path);
            throw;
        }
    }
}