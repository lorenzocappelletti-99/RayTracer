using Xunit;

namespace Trace;

public class HdrImage
{
    public int Width { get; set; }
    public int Height { get; set; }
    public Color[] Pixels { get; set; }
    
    // Constructor that initializes the image with a width and height
    public HdrImage(int width = 0, int height = 0)
    {
        Width = width;
        Height = height;
        Pixels = new Color[Width * Height];

        // Initialize all pixels to black
        for (var i = 0; i < Pixels.Length; i++)
        {
            Pixels[i] = new Color();
        }
    }

    public bool valid_coordinates(int x, int y)
    {
        /*
        // Check if x is out of bounds
        if (x < 0 || x >= Width)
        {
            throw new ArgumentOutOfRangeException(nameof(x), "The x coordinate is out of bounds.");
        }
        // Check if y is out of bounds
        if (y < 0 || y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y), "The y coordinate is out of bounds.");
        } 
        */
        return x >= 0  && x < Width && y >= 0 && y < Height;
    }
    public int pixel_offset(int x, int y)
    {
        return y * Width + x;
    }    
    public Color GetPixel(int x, int y)
    {
        Assert.True(valid_coordinates(x, y));
        // Return the color of the pixel
        return Pixels[pixel_offset(x, y)];
    }

    public void SetPixel(int x, int y, Color newColor)
    {
        Assert.True(valid_coordinates(x, y));
        // Set the new color for the pixel
        Pixels[pixel_offset(x,y)] = newColor;
    }

}