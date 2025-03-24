using System.Text;
using System.IO;
using Xunit.Abstractions;

namespace Trace.Tests;


public class HdrImageTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public HdrImageTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TestHdrImageConstruction()
    {
        var img = new HdrImage(7,4);
        Assert.Equal(7, img.Width);
        Assert.Equal(4, img.Height);
        Assert.Equal(7*4, img.Pixels.Length);
    }
    
    [Fact]
    public void TestPixelBounds()
    {
        var img = new HdrImage(7,4);
        Assert.True(img.valid_coordinates(0, 0), "invalid coordinates");
        Assert.True(img.valid_coordinates(6, 3), "invalid coordinates");
        Assert.False(img.valid_coordinates(0, -1), "Invalid coordinates (negative y)");
        Assert.False(img.valid_coordinates(-1, 0), "Invalid coordinates (negative x)");
        Assert.False(img.valid_coordinates(7, 0), "Invalid coordinates (x out of range)");
        Assert.False(img.valid_coordinates(0, 4), "Invalid coordinates (y out of range)");

         }
    [Fact]
    public void TestPixelOffset()
    {
        var img = new HdrImage(7,4);
        Assert.True(img.pixel_offset(3, 2) == 17, "Pixel offset doesn't work properly.");
        Assert.True(img.pixel_offset(6, 3) == 7 * 4 -1, "Pixel offset doesn't work properly.");

    }

    [Fact]
    public void TestWritePfm()
    {
        var memoryStream = new MemoryStream();

        var img = new HdrImage(3, 2);
        
        img.SetPixel(0, 0, new Color(1.0e1f, 2.0e1f, 3.0e1f));
        img.SetPixel(1, 0, new Color(4.0e1f, 5.0e1f, 6.0e1f));
        img.SetPixel(2, 0, new Color(7.0e1f, 8.0e1f, 9.0e1f));
        img.SetPixel(0, 1, new Color(1.0e2f, 2.0e2f, 3.0e2f));
        img.SetPixel(1, 1, new Color(4.0e2f, 5.0e2f, 6.0e2f));
        img.SetPixel(2, 1, new Color(7.0e2f, 8.0e2f, 9.0e2f));

        byte[] referenceBytes =
        [
            0x50, 0x46, 0x0A, 0x33, 0x20, 0x32, 0x0A, 0x2D, 0x31, 0x0A, 0x41, 0x20, 0x00, 0x00, 0x41, 0xA0,
            0x00, 0x00, 0x41, 0xF0, 0x00, 0x00, 0x42, 0x20, 0x00, 0x00, 0x42, 0x48, 0x00, 0x00, 0x42, 0x70,
            0x00, 0x00, 0x42, 0x8c, 0x00, 0x00, 0x42, 0xa0, 0x00, 0x00, 0x42, 0xb4, 0x00, 0x00, 0x42, 0xc8,
            0x00, 0x00, 0x43, 0x48, 0x00, 0x00, 0x43, 0x96, 0x00, 0x00, 0x43, 0xc8, 0x00, 0x00, 0x43, 0xfa,
            0x00, 0x00, 0x44, 0x16, 0x00, 0x00, 0x44, 0x2f, 0x00, 0x00, 0x44, 0x48, 0x00, 0x00, 0x44, 0x61,
            0x00, 0x00,  
            ];

        using var buf = new MemoryStream();
        img.WritePfm(buf, false); // Write PFM data to MemoryStream
        var resultBytes = buf.ToArray(); // Get written data
   
        // Check if output matches referenceBytes
        var isEqual = referenceBytes.SequenceEqual(resultBytes);
        
        Assert.True(resultBytes.SequenceEqual(referenceBytes));
    }

    /*
    [Fact]
    public void TestReadPfm()
    {
        // 1. Crea il file PFM in memoria
        var header = Encoding.ASCII.GetBytes("PF\n2 2\n-1.0\n");

        // Dati binari per un'immagine 2x2
        var pixelData = new byte[]
        {
            // Prima riga (bottom)
            0x3F, 0x80, 0x00, 0x00, // 1.0 (R)
            0x00, 0x00, 0x00, 0x00, // 0.0 (G)
            0x00, 0x00, 0x00, 0x00, // 0.0 (B)
            0x00, 0x00, 0x00, 0x00, // 0.0 (R)
            0x3F, 0x80, 0x00, 0x00, // 1.0 (G)
            0x00, 0x00, 0x00, 0x00, // 0.0 (B)

            // Seconda riga (top)
            0x00, 0x00, 0x00, 0x00, // 0.0 (R)
            0x00, 0x00, 0x00, 0x00, // 0.0 (G)
            0x3F, 0x80, 0x00, 0x00, // 1.0 (B)
            0x3F, 0x80, 0x00, 0x00, // 1.0 (R)
            0x3F, 0x80, 0x00, 0x00, // 1.0 (G)
            0x00, 0x00, 0x00, 0x00  // 0.0 (B)
        };

        // Combina header e dati binari
        var pfmData = new byte[header.Length + pixelData.Length];
        Buffer.BlockCopy(header, 0, pfmData, 0, header.Length);
        Buffer.BlockCopy(pixelData, 0, pfmData, header.Length, pixelData.Length);

        // 2. Crea uno stream dai dati binari
        using var stream = new MemoryStream(pfmData);

        // 3. Chiama la funzione ReadPfm
        var image = HdrImage.ReadPfm(stream);

        // 4. Verifica le dimensioni dell'immagine
        Assert.Equal(2, image.Width);
        Assert.Equal(2, image.Height);

        // 5. Verifica i pixel
        Assert.Equal(new Color(1.0f, 0.0f, 0.0f), image.GetPixel(0, 0)); // Top-left (rosso)
        Assert.Equal(new Color(0.0f, 1.0f, 0.0f), image.GetPixel(1, 0)); // Top-right (verde)
        Assert.Equal(new Color(0.0f, 0.0f, 1.0f), image.GetPixel(0, 1)); // Bottom-left (blu)
        Assert.Equal(new Color(1.0f, 1.0f, 0.0f), image.GetPixel(1, 1)); // Bottom-right (giallo)
    }
    */
    
        /* $ xxd reference_be.pfm
    00000000: 5046 0a33 2032 0a31 2e30 0a42 c800 0043  PF.3 2.1.0.B...C
    00000010: 4800 0043 9600 0043 c800 0043 fa00 0044  H..C...C...C...D
    00000020: 1600 0044 2f00 0044 4800 0044 6100 0041  ...D/..DH..Da..A
    00000030: 2000 0041 a000 0041 f000 0042 2000 0042   ..A...A...B ..B
    00000040: 4800 0042 7000 0042 8c00 0042 a000 0042  H..Bp..B...B...B
    00000050: b400 00
         */
        /* $ xxd reference_le.pfm
    00000000: 5046 0a33 2032 0a2d 312e 300a 0000 c842  PF.3 2.-1.0....B
    00000010: 0000 4843 0000 9643 0000 c843 0000 fa43  ..HC...C...C...C
    00000020: 0000 1644 0000 2f44 0000 4844 0000 6144  ...D../D..HD..aD
    00000030: 0000 2041 0000 a041 0000 f041 0000 2042  .. A...A...A.. B
    00000040: 0000 4842 0000 7042 0000 8c42 0000 a042  ..HB..pB...B...B
    00000050: 0000 b442
         */
}