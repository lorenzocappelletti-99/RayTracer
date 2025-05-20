/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using Xunit;
using System.Text;
using System.Globalization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Trace;

public class HdrImage
{
    public int Width { get; set; }
    public int Height { get; set; }
    public Color[] Pixels { get; set; }

    
    /***********************************************************
     *                                                         *
     *                       CONSTRUCTORS                     *
     *                                                         *
     ***********************************************************/
    
    /// <summary>
    /// Constructs HdrImage type with width and height
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
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

    /// <summary>
    /// Constructs HdrImage type by reading it from a PFM, RGB file
    /// </summary>
    /// <param name="filePath"></param>
    public HdrImage(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);
        var tempImage = ReadPfm(fileStream);
        this.Width = tempImage.Width;
        this.Height = tempImage.Height;
        this.Pixels = tempImage.Pixels;
    }
    
    /***********************************************************
     *                                                         *
     *                      PIXEL HANDLING                     *
     *                                                         *
     ***********************************************************/


    /// <summary>
    /// Internal method, checks validity of variables. Good for tests.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
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
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    /// <summary>
    /// Internal method, implement pixel search in a streamed matrix.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int pixel_offset(int x, int y)
    {
        return y * Width + x;
    }

    /// <summary>
    /// Returns pixel, Color type. Checks validity of position requested, uses pixel_offset method.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Color GetPixel(int x, int y)
    {
        Assert.True(valid_coordinates(x, y), $"Tried to READ out-of-range pixel: x = {x}, y = {y}");

        return Pixels[pixel_offset(x, y)];
    }

    /// <summary>
    /// Sets a pixel. Checks validity of position requested, uses pixel_offset method.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="color"></param>
    public void SetPixel(int x, int y, Color color)
    {
        Assert.True(valid_coordinates(x, y), $"Tried to SET out-of-range pixel: x = {x}, y = {y}");
        Pixels[pixel_offset(x, y)] = color;
    }

    public void SetAllPixels(Color newColor)
    {
        for (int i = 0; i < Pixels.Length; i++)
            Pixels[i] = newColor;
    }
    
    
    /***********************************************************
     *                                                         *
     *                      READ/WRITE PFM                     *
     *                                                         *
     ***********************************************************/
    
    /// <summary>
    /// Support function. Reads a line of bytes from stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static byte[] ReadLineBytes(Stream stream)
    {
        List<byte> bytes = [];
        int b;
        while ((b = stream.ReadByte()) != -1 && b != '\n')
        {
            if (b != '\r') bytes.Add((byte)b); // Ignore '\r' in Windows-style line endings
        }

        return bytes.ToArray();
    }

    /// <summary>
    /// Support function. Reads float with specified endianness
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="isLittleEndian"></param>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public static float ReadFloat(BinaryReader reader, bool isLittleEndian)
    {
        byte[] bytes = reader.ReadBytes(4);
        if (bytes.Length != 4) throw new EndOfStreamException("Unexpected end of stream while reading float.");
        if (BitConverter.IsLittleEndian != isLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToSingle(bytes, 0);
    }


    /// <summary>
    /// Support function. Writes line of bytes in stream.
    /// </summary>
    /// <param name="outputStream"></param>
    /// <param name="value"></param>
    /// <param name="isLittleEndian"></param>
    private static void WriteFloat(Stream outputStream, float value, bool isLittleEndian = true)
    {
        var bytes = BitConverter.GetBytes(value);

        // If machine is little-endian and big-endian is needed, invert bytes
        if (BitConverter.IsLittleEndian != isLittleEndian) Array.Reverse(bytes);

        outputStream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Endianness must be bool. The method will convert it to float when printing on file. true -> Little endianness. 
    /// </summary>
    /// <param name="outputStream"></param>
    /// <param name="endianness"></param>
    public void WritePfm(Stream outputStream, bool endianness = true)
    {
        using var writer = new BinaryWriter(outputStream, Encoding.ASCII, leaveOpen: true);

        // Header
        writer.Write(Encoding.ASCII.GetBytes("PF\n")); // "PF" means an RGB image
        writer.Write(Encoding.ASCII.GetBytes($"{Width} {Height}\n"));

        float floatEndianness;
        if (endianness)
        {
            floatEndianness = -1.0f;
        }
        else
        {
            floatEndianness = 1.0f;
        }

        writer.Write(Encoding.ASCII.GetBytes($"{floatEndianness.ToString("0.0", CultureInfo.InvariantCulture)}\n"));
        // watch out! here the .0 after the +-1 is written. Crucial detail.


        // Pixels are written (bottom-to-up, left-to-right). Columns first, then lines.
        for (var y = Height - 1; y >= 0; y--)
        {
            for (var x = 0; x < Width; x++)
            {
                var color = this.GetPixel(x, y);
                WriteFloat(outputStream, color.R, endianness);
                WriteFloat(outputStream, color.G, endianness);
                WriteFloat(outputStream, color.B, endianness);
            }
        }
    }

    /// <summary>
    /// Reads pfm. Returns the HdrImage of that pfm file.
    /// </summary>
    /// <param name="inputStream"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public static HdrImage ReadPfm(Stream inputStream)
    {
        // 1. Read and validate "PF" header
        byte[] line1Bytes = ReadLineBytes(inputStream);
        string line1 = Encoding.ASCII.GetString(line1Bytes).Trim();
        if (line1 != "PF")
            throw new InvalidDataException("Invalid PFM format: missing 'PF' in the first line.");

        // 2. Read width and height
        byte[] line2Bytes = ReadLineBytes(inputStream);
        string line2 = Encoding.ASCII.GetString(line2Bytes).Trim();
        string[] whParts = line2.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (whParts.Length != 2 ||
            !int.TryParse(whParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int width) ||
            !int.TryParse(whParts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int height) ||
            width <= 0 || height <= 0)
            throw new InvalidDataException("Invalid PFM format: incorrect image dimensions.");

        // 3. Read endianness (-1.0 = little-endian, 1.0 = big-endian)
        byte[] line3Bytes = ReadLineBytes(inputStream);
        string line3 = Encoding.ASCII.GetString(line3Bytes).Trim();
        if (!float.TryParse(line3, NumberStyles.Float, CultureInfo.InvariantCulture, out float endianValue))
            throw new InvalidDataException("Invalid PFM format: malformed endianness value.");
        bool isLittleEndian = endianValue < 0;

        // 4. Verify data length
        long expectedBytes = 12L * width * height;
        if (inputStream.Length - inputStream.Position < expectedBytes)
            throw new InvalidDataException("Insufficient data in PFM file.");

        // 5. Create image and fill pixels
        HdrImage image = new HdrImage(width, height);
        using var reader = new BinaryReader(inputStream, Encoding.ASCII, leaveOpen: true);
        // PFM pixels are written bottom-to-top, convert to top-to-bottom
        for (int yPfm = 0; yPfm < height; yPfm++)
        {
            int yHdr = height - 1 - yPfm; // Convert to HdrImage coordinates (top-to-bottom)
            for (int x = 0; x < width; x++)
            {
                float r = ReadFloat(reader, isLittleEndian);
                float g = ReadFloat(reader, isLittleEndian);
                float b = ReadFloat(reader, isLittleEndian);
                image.SetPixel(x, yHdr, new Color(r, g, b));
            }
        }

        return image;
    }
    
    
    /***********************************************************
     *                                                         *
     *                      LDR CONVERSION                     *
     *                                                         *
     ***********************************************************/

    /// <summary>
    /// Reads, Normalizes and Clamps pfmFile. Saves in working directory the converted LDR.
    /// If file extension is not specified converts in png. This is decided in <see cref="Myraytracer.ParametersDemo.OutputLdrFileName"/>
    /// Automatically converts in jpg or png depending on given filename extension.
    /// </summary>
    /// <param name="pfmPath"></param>
    /// <param name="filePath"></param>
    /// <param name="gamma"></param>
    /// <param name="factor"></param>
    public static void write_ldr_image(Stream pfmPath, string filePath, float gamma=1.0f, float factor=1.0f)
    {
        try
        {
            var img = ReadPfm(pfmPath);
            img.NormalizeImage(factor);
            img.ClampImage();
            using var image = new Image<Rgb24>(img.Width, img.Height);

            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    var col = img.GetPixel(x, y);
                    //var r = (byte)Math.Clamp(col.R * 255, 0, 255);
                    //var g = (byte)Math.Clamp(col.G * 255, 0, 255);
                    //var b = (byte)Math.Clamp(col.B * 255, 0, 255);
                    var r = (byte)Math.Clamp(Math.Pow(col.R, 1.0f/gamma) * 255, 0, 255);
                    var g = (byte)Math.Clamp(Math.Pow(col.G, 1.0f/gamma) * 255, 0, 255);
                    var b = (byte)Math.Clamp(Math.Pow(col.B, 1.0f/gamma) * 255, 0, 255);
                    

                    image[x, y] = new Rgb24(r, g, b);
                }
            }

            if(filePath.Contains("JPG", StringComparison.OrdinalIgnoreCase)) image.SaveAsJpeg(filePath);
            if(filePath.Contains("PNG", StringComparison.OrdinalIgnoreCase)) image.SaveAsPng(filePath);
            

            Console.WriteLine($"Conversion complete: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during conversion: {ex.Message}");
        }
    }
    

    /***********************************************************
     *                                                         *
     *                       TONE MAPPING                      *
     *                                                         *
     ***********************************************************/
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="delta"></param>
    /// <returns></returns>
    public float AverageLuminosity(double delta = 1e-10d)
    {
        var cumsum = 0.0d;
        foreach (var pixel in Pixels)
            cumsum += Math.Log10(delta + pixel.Luminosity());

        return (float)Math.Pow(10, cumsum / Pixels.Length);
    }

    /// <summary>
    /// Normalizes pixel values
    /// </summary>
    /// <param name="factor"></param>
    /// <param name="luminosity"></param>
    public void NormalizeImage(float factor, float? luminosity = null)
    {
        var lum = luminosity ?? AverageLuminosity();
        for (int i = 0; i < Pixels.Length; i++)
        {
            Pixels[i] = Pixels[i] * (factor / lum);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static float Clamp(float x)
    {
        return x / (1 + x);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void ClampImage()
    {
        for (var i = 0; i < Pixels.Length; i++)
        {
            Pixels[i].R = Clamp(Pixels[i].R);
            Pixels[i].G = Clamp(Pixels[i].G);
            Pixels[i].B = Clamp(Pixels[i].B);
        }
    }
}
    
