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
    
}