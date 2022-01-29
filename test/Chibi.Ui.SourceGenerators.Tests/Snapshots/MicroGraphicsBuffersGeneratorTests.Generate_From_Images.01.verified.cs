﻿//HintName: Icons.Buffers.cs
namespace Chibi.Ui.Tests
{
    using Meadow.Foundation.Graphics.Buffers;

    public partial class Icons
    {
            public Buffer1bpp Play16x16 { get; } = new Buffer1bpp(16, 16, new byte[]
            {
                0,0,255,255,3,3,7,7,14,30,60,56,112,240,224,128,0,0,255,255,192,192,224,224,112,120,60,28,14,15,7,1
            });

            public Buffer1bpp Settings16x16 { get; } = new Buffer1bpp(16, 16, new byte[]
            {
                112,124,252,252,252,252,255,127,127,255,252,252,252,252,124,112,14,62,63,63,63,63,255,254,254,255,63,63,63,63,62,14
            });

            public Buffer1bpp Test16x16 { get; } = new Buffer1bpp(16, 16, new byte[]
            {
                0,0,255,255,3,3,243,243,243,255,31,31,254,252,0,0,0,0,255,255,216,248,251,255,223,219,216,216,255,255,0,0
            });

            public Buffer1bpp Play32x32 { get; } = new Buffer1bpp(32, 32, new byte[]
            {
                0,0,0,0,248,254,254,63,15,15,7,7,15,31,31,62,124,248,248,240,224,224,192,128,0,0,0,0,0,0,0,0,0,0,0,0,255,255,255,0,0,0,0,0,0,0,0,0,0,0,0,1,3,7,7,15,31,31,126,252,248,240,128,0,0,0,0,0,255,255,255,0,0,0,0,0,0,0,0,0,0,0,0,128,192,224,224,240,248,248,126,63,31,15,1,0,0,0,0,0,31,127,127,252,240,240,224,224,240,248,248,124,62,31,31,15,7,7,3,1,0,0,0,0,0,0,0,0
            });

            public Buffer1bpp Settings32x32 { get; } = new Buffer1bpp(32, 32, new byte[]
            {
                0,0,192,240,240,224,224,192,192,224,224,240,255,255,255,255,255,255,255,255,240,224,224,192,192,224,224,240,240,192,0,0,14,31,63,63,255,255,255,255,255,255,255,255,255,63,31,31,31,31,63,255,255,255,255,255,255,255,255,255,63,63,31,14,112,248,252,252,255,255,255,255,255,255,255,255,255,252,248,248,248,248,252,255,255,255,255,255,255,255,255,255,252,252,248,112,0,0,3,15,15,7,7,3,3,7,7,15,255,255,255,255,255,255,255,255,15,7,7,3,3,7,7,15,15,3,0,0
            });

            public Buffer1bpp Test32x32 { get; } = new Buffer1bpp(32, 32, new byte[]
            {
                0,0,0,0,248,254,254,63,15,15,7,7,7,7,7,7,7,7,255,255,255,255,223,254,252,248,240,224,0,0,0,0,0,0,0,0,255,255,255,0,0,0,0,0,252,254,254,222,222,254,254,253,3,3,3,3,3,255,255,255,0,0,0,0,0,0,0,0,255,255,255,0,0,192,192,192,143,207,239,227,227,239,143,207,192,192,192,0,0,255,255,255,0,0,0,0,0,0,0,0,31,127,127,252,240,243,231,231,231,231,231,227,225,224,225,227,227,227,243,240,252,127,127,31,0,0,0,0
            });

    }
}