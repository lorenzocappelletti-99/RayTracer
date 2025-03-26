using Xunit;
using System.Text;
using System.Globalization;

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
    
    /// <summary>
    /// Constructs an HdrImage type by reading it from a PFM, RGB file
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
        return Pixels[pixel_offset(x, y)];
    }

    public void SetPixel(int x, int y, Color newColor)
    {
        Assert.True(valid_coordinates(x, y));
        Pixels[pixel_offset(x,y)] = newColor;
    }
    
    
    private static void WriteFloat(Stream outputStream, float value, bool isLittleEndian = true)
    {
        var bytes = BitConverter.GetBytes(value);

        // If machine is little-endian and big-endian is needed, invert bytes
        if (BitConverter.IsLittleEndian != isLittleEndian) Array.Reverse(bytes);

        outputStream.Write(bytes, 0, bytes.Length);
    }

    public void SavePfm(Stream outputStream)
    {
        
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
        if (endianness == true)
        {
            floatEndianness = -1.0f;
        }
        else
        {
            floatEndianness = 1.0f;
        }

        // watch out! here the .0 after the +-1 is written. Crucial detail.
        writer.Write(Encoding.ASCII.GetBytes($"{floatEndianness:0.0}\n"));

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
/// 
/// </summary>
/// <param name="inputStream"></param>
/// <returns></returns>
/// <exception cref="InvalidDataException"></exception>
    public HdrImage ReadPfm(Stream inputStream)
        {
            // 1. Read and validate "PF" header
            var line1Bytes = ReadLineBytes(inputStream);
            var line1 = Encoding.ASCII.GetString(line1Bytes).Trim();
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

        // Helper to read a line of bytes from the stream
        byte[] ReadLineBytes(Stream stream)
        {
            List<byte> bytes = [];
            int b;
            while ((b = stream.ReadByte()) != -1 && b != '\n')
            {
                if (b != '\r') bytes.Add((byte)b); // Ignore '\r' in Windows-style line endings
            }

            return bytes.ToArray();
        }

        // Helper to read float with specified endianness
        float ReadFloat(BinaryReader reader, bool isLittleEndian)
        {
            var bytes = reader.ReadBytes(4);
            if (bytes.Length != 4) throw new EndOfStreamException("Unexpected end of stream while reading float.");
            if (BitConverter.IsLittleEndian != isLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }
    }

