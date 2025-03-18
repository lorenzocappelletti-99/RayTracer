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
        for (int i = 0; i < Pixels.Length; i++)
        {
            Pixels[i] = new Color();
        }
    }
    
    public Color GetPixel(int x, int y)
    {
        // Verify that the coordinates are valid
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            throw new Exception("The pixel coordinates are out of bounds.");
        }

        // Return the color of the pixel
        return Pixels[y * Width + x];
    }
    
    // Sets the color of a specific pixel by coordinates
    public void SetPixel(int x, int y, Color newColor)
    {
        // Verify that the coordinates are valid
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            throw new Exception("The pixel coordinates are out of bounds.");
        }

        // Set the new color for the pixel
        Pixels[y * Width + x] = newColor;
    }
}