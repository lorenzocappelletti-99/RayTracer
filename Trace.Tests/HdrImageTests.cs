/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using Xunit;
using FileStream = System.IO.FileStream;

namespace Trace.Tests;


public class HdrImageTests
{

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
        //_testOutputHelper.WriteLine($"pixel:{img.GetPixel(1,0)}");

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
        var img = new HdrImage(3, 2);
        img.SetPixel(0, 0, new Color(1.0e1f, 2.0e1f, 3.0e1f));
        img.SetPixel(1, 0, new Color(4.0e1f, 5.0e1f, 6.0e1f));
        img.SetPixel(2, 0, new Color(7.0e1f, 8.0e1f, 9.0e1f));
        img.SetPixel(0, 1, new Color(1.0e2f, 2.0e2f, 3.0e2f));
        img.SetPixel(1, 1, new Color(4.0e2f, 5.0e2f, 6.0e2f));
        img.SetPixel(2, 1, new Color(7.0e2f, 8.0e2f, 9.0e2f));
        
/*
       byte[] referenceBytes = [
               0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a,
    0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43,
    0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
    0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
    0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
    0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
    0x00, 0x00, 0x8c, 0x42, 0x00, 0x00, 0xa0, 0x42, 0x00, 0x00, 0xb4, 0x42
       ];

*/
       byte[] referenceBytes;

       using (var fs = new FileStream("../../../reference_le.pfm", FileMode.Open, FileAccess.Read))
       {
           referenceBytes = new byte[fs.Length];
           fs.ReadExactly(referenceBytes, 0, (int)fs.Length);
       }

        
        using var buf = new MemoryStream();
        img.WritePfm(buf);
        var resultBytes = buf.ToArray();

        for (var i = 0; i < referenceBytes.Length; i++)
        {
            Assert.True(referenceBytes[i] == resultBytes[i]);
        }
    }
    
    
    [Fact]
    public void TestReadPfm_LE()
    {
        // 1. Ottieni il percorso della directory del progetto
        var projectDir = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
        if (projectDir == null) throw new Exception("Project directory not found");
    
        // 2. Costruisci il percorso completo del file
        var pfmFilePath = Path.Combine(projectDir, "reference_le.pfm");
    
        // 3. Verifica che il file esista
        if (!File.Exists(pfmFilePath))
        {
            throw new FileNotFoundException($"PFM file not found at: {pfmFilePath}");
        }

        // 4. Leggi e verifica l'immagine
        using var stream = File.OpenRead(pfmFilePath);
        HdrImage image = HdrImage.ReadPfm(stream);

        // 3. Verifica le dimensioni
        Assert.Equal(3, image.Width);
        Assert.Equal(2, image.Height);
        
        /*
        for (int j = 0; j<2; j++)
            for (int i = 0; i < 3; i++)
                _testOutputHelper.WriteLine($"pixel({i},{j}): {image.GetPixel(i,j)}");
       */ 
        
        Assert.True(Color.are_close_colors(image.GetPixel(0, 0), new Color(1.0e1f, 2.0e1f, 3.0e1f)));
        Assert.True(Color.are_close_colors(image.GetPixel(1, 0), new Color(4.0e1f, 5.0e1f, 6.0e1f)));
        Assert.True(Color.are_close_colors(image.GetPixel(2, 0), new Color(7.0e1f, 8.0e1f, 9.0e1f)));
        Assert.True(Color.are_close_colors(image.GetPixel(0, 1), new Color(1.0e2f, 2.0e2f, 3.0e2f)));
        Assert.True(Color.are_close_colors(image.GetPixel(1, 1), new Color(4.0e2f, 5.0e2f, 6.0e2f)));
        Assert.True(Color.are_close_colors(image.GetPixel(2, 1), new Color(7.0e2f, 8.0e2f, 9.0e2f)));
    }
    
    [Fact]
    public void TestReadPfm_BE()
    {
        // 1. get path project directory
        var projectDir = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
        if (projectDir == null) throw new Exception("Project directory not found");
    
        // 2. build complete path
        var pfmFilePath = Path.Combine(projectDir, "reference_be.pfm");
    
        // 3. check file existence
        if (!File.Exists(pfmFilePath))
        {
            throw new FileNotFoundException($"PFM file not found at: {pfmFilePath}");
        }

        // 4. read and verify image
        using var stream = File.OpenRead(pfmFilePath);
        HdrImage image = HdrImage.ReadPfm(stream);

        // 5. check dimensions
        Assert.Equal(3, image.Width);
        Assert.Equal(2, image.Height);
    
        // 6. check pixels (aggiusta questi valori in base al contenuto reale del tuo file BE)
        Assert.True(Color.are_close_colors(image.GetPixel(0, 0), new Color(1.0e1f, 2.0e1f, 3.0e1f)));
        Assert.True(Color.are_close_colors(image.GetPixel(1, 0), new Color(4.0e1f, 5.0e1f, 6.0e1f)));
        Assert.True(Color.are_close_colors(image.GetPixel(2, 0), new Color(7.0e1f, 8.0e1f, 9.0e1f)));
        Assert.True(Color.are_close_colors(image.GetPixel(0, 1), new Color(1.0e2f, 2.0e2f, 3.0e2f)));
        Assert.True(Color.are_close_colors(image.GetPixel(1, 1), new Color(4.0e2f, 5.0e2f, 6.0e2f)));
        Assert.True(Color.are_close_colors(image.GetPixel(2, 1), new Color(7.0e2f, 8.0e2f, 9.0e2f)));
    }


    [Fact]
    public void TestAverageLuminosity()
    {
        var img = new HdrImage(2,1);
        
        img.SetPixel(0,0, new Color(5f, 10f, 15f));
        img.SetPixel(1,0, new Color(5e2f, 1e3f, 1.5e3f));
        
        //_testOutputHelper.WriteLine($"Average Luminosity: {img.AverageLuminosity()}  ");
        var expectedAvLum = 100f;
        var actualAvLum = img.AverageLuminosity();
        // Assert con epsilon
        float epsilon = 1e-5f;
        Assert.True(Math.Abs(expectedAvLum - actualAvLum) < epsilon, 
            $"Expected {expectedAvLum}, but got {actualAvLum}");
        
    }
    
    [Fact]
    public void TestAverageLuminosity_BP() //test for black pixel
    {
        var img = new HdrImage(2,1);
        
        img.SetPixel(0,0, new Color(5f, 10f, 15f));
        img.SetPixel(1,0, new Color());
        
        //_testOutputHelper.WriteLine($"Average Luminosity: {img.AverageLuminosity()}  ");
        var expectedAvLum = 1e-1f;
        var actualAvLum = img.AverageLuminosity(1e-3d);
        // Assert con epsilon
        float epsilon = 1e-5f;
        Assert.True(Math.Abs(expectedAvLum - actualAvLum) < epsilon, 
            $"Expected {expectedAvLum}, but got {actualAvLum}");
    }

    [Fact]
    public void TestNormalizeImage()
    {
        var img = new HdrImage(2,1);
        
        img.SetPixel(0,0, new Color(5f, 10f, 15f));
        img.SetPixel(1,0, new Color(5e2f, 1e3f, 1.5e3f));
        
        img.NormalizeImage(1e3f,1e2f);
        
        Assert.True(Color.are_close_colors(img.GetPixel(0, 0), new Color(0.5e2f, 1.0e2f, 1.5e2f)));
        Assert.True(Color.are_close_colors(img.GetPixel(1, 0), new Color(0.5e4f, 1.0e4f, 1.5e4f)));
    }

    [Fact]
    public void TestClampImage()
    {
        var img = new HdrImage(2,1);
        
        img.SetPixel(0,0, new Color(5f, 10f, 15f));
        img.SetPixel(1,0, new Color(5e2f, 1e3f, 1.5e3f));
        
        img.ClampImage();

        foreach (var pix in img.Pixels)
        {
            Assert.True(pix.R is >= 0 and <= 1);
            Assert.True(pix.G is >= 0 and <= 1);
            Assert.True(pix.B is >= 0 and <= 1);
        }
    }

    
    
    
    
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