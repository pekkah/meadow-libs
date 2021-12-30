using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;

namespace Chibi.Ui.Simple;

public class WriteableBitmapDisplay : IGraphicsDisplay
{
    private readonly WriteableBitmap _targetBitmap;
    private readonly BufferRgb888 _backbuffer;
    private readonly Color _clearColor;

    public WriteableBitmapDisplay(WriteableBitmap targetBitmap, Color? clearColor = null)
    {
        _clearColor = clearColor ?? Color.Black;
        _targetBitmap = targetBitmap;
        _backbuffer = new BufferRgb888(Width, Height);
    }

    public void Show()
    {
        _targetBitmap.WritePixels(
            new Int32Rect(0, 0, Width, Height),
            _backbuffer.Buffer,
            _targetBitmap.BackBufferStride, 
            0,
            0);
    }

    public void Show(int left, int top, int right, int bottom)
    {
        _targetBitmap.WritePixels(
            new Int32Rect(left, top, right, bottom),
            _backbuffer.Buffer,
            _targetBitmap.BackBufferStride,
            0,
            0);
    }

    public void Clear(bool updateDisplay = false)
    {
        _backbuffer.Clear();

        if (updateDisplay)
            Show();
    }

    public void Fill(Color fillColor, bool updateDisplay = false)
    {
        var b = Color2Byte(fillColor);
        _backbuffer.Fill(fillColor);

        if (updateDisplay)
            Show();
    }

    public void Fill(int x, int y, int width, int height, Color fillColor)
    {
        for (int tx = x; tx < width; tx++)
        {
            for(int ty = y; ty < height; ty++)
                DrawPixel(tx, ty, fillColor);
        }
    }

    public void DrawPixel(int x, int y, Color color)
    {
        if (IgnoreOutOfBoundsPixels)
        {
            if (x > Width || x < 0)
                return;

            if (y > Height || y < 0)
                return;
        }

        _backbuffer.SetPixel(x, y, color);
    }

    public void DrawPixel(int x, int y, bool colored)
    {
        if (IgnoreOutOfBoundsPixels)
        {
            if (x > Width || x < 0)
                return;

            if (y > Height || y < 0)
                return;
        }

        DrawPixel(x, y, colored ? Color.White : Color.Black);
    }

    public void InvertPixel(int x, int y)
    {
        if (IgnoreOutOfBoundsPixels)
        {
            if (x > Width || x < 0)
                return;

            if (y > Height || y < 0)
                return;
        }

        var pixel = _backbuffer.GetPixel(x, y);
        _backbuffer.SetPixel(x, y, pixel);
    }

    public void DrawBuffer(int x, int y, IDisplayBuffer displayBuffer)
    {
        for (var sx = 0; sx < displayBuffer.Width; sx++)
        for (var sy = 0; sy < displayBuffer.Height; sy++)
        {
            var sourcePixel = displayBuffer.GetPixel(sx, sy);
            DrawPixel(x + sx, y + sy, sourcePixel);
        }
    }

    public ColorType ColorMode => ColorType.Format24bppRgb888;

    public int Width => _targetBitmap.PixelWidth;

    public int Height => _targetBitmap.PixelHeight;

    public bool IgnoreOutOfBoundsPixels { get; set; }

    private byte[] Color2Byte(Color c)
    {
        return new[]
        {
            c.B,
            c.G,
            c.R,
            //c.A
        };
    }
}