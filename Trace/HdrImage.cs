using Xunit;
using System.Text;

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
    
    
    /*
 * def _write_float(stream, value):
# Meaning of "<f":
# "<": little endian
# "f": single-precision floating point value (32 bit)
stream.write(struct.pack("<f", value))
 */
    private static void WriteFloat(Stream outputStream, float value, bool littleEndian = false)
    {
        var bytes = BitConverter.GetBytes(value);

        // Se la macchina è little-endian ma vogliamo big-endian, invertiamo i byte
        if (BitConverter.IsLittleEndian != littleEndian)
        {
            Array.Reverse(bytes);
        }

        outputStream.Write(bytes, 0, bytes.Length);
    }

    public void SavePfm(Stream outputStream)
    {
        
    }

    public void WritePfm(HdrImage img, bool endianness = false)//(, int endianness=Endianness.LITTLE_ENDIAN)
    {
        string filePath = @"C:\Users\pietr\OneDrive\Desktop\fisica\magistrale\SEM_2\FotoReal\Myraytracer\Myraytracer\output.pfm";

        using FileStream fileStream = File.OpenWrite(filePath);
        using BinaryWriter writer = new BinaryWriter(fileStream, Encoding.ASCII);

        // Header
        writer.Write(Encoding.ASCII.GetBytes("PF\n")); // "PF" indica un'immagine RGB
        writer.Write(Encoding.ASCII.GetBytes($"{img.Width} {img.Height}\n"));

        var floatEndiannes = -1.0f;
        if (endianness == true){ floatEndiannes = 1.0f; }
        writer.Write(Encoding.ASCII.GetBytes($"{floatEndiannes}\n"));

        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                var color = this.GetPixel(x, y);
                WriteFloat(fileStream, color.R);
                WriteFloat(fileStream, color.G);
                WriteFloat(fileStream, color.B);
            }
        }
        
        /*
         *     # Write the image (bottom-to-up, left-to-right)
    for y in reversed(range(self.height)):
        for x in range(self.width):
            color = self.get_pixel(x, y)
            _write_float(stream, color.r, endianness)
            _write_float(stream, color.g, endianness)
            _write_float(stream, color.b, endianness)
         */
    }

    /*
    def write_pfm(self, 
    ):
    endianness_str = "-1.0" if endianness == Endianness.LITTLE_ENDIAN else "1.0"

    # The PFM header, as a Python string (UTF-8)
    header = f"PF\n{self.width} {self.height}\n{endianness_str}\n"

    # Convert the header into a sequence of bytes
    stream.write(header.encode("ascii"))

    # Write the image (bottom-to-up, left-to-right)
    for y in reversed(range(self.height)):
        for x in range(self.width):
            color = self.get_pixel(x, y)
            _write_float(stream, color.r, endianness)
            _write_float(stream, color.g, endianness)
            _write_float(stream, color.b, endianness)
     */

}