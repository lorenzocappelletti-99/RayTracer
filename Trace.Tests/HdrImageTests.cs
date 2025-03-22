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

         }
    [Fact]
    public void TestPixelOffset()
    {
        var img = new HdrImage(7,4);
        Assert.True(img.pixel_offset(3, 2) == 17, "Pixel offset doesn't work properly.");
        Assert.True(img.pixel_offset(6, 3) == 7 * 4 -1, "Pixel offset doesn't work properly.");

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