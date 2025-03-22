/*
namespace Trace;

static class Program
{
    static void Main(string[] _) // the parameter string is unused
    {
        ///////// test Color.cs /////////////////////
        
        // Create some color objects for testing
        var color1 = new Color(0.5f, 0.6f, 0.7f);
        var color2 = new Color(0.2f, 0.3f, 0.4f);
        var color3 = new Color(0.7f, 0.9f, 1.0f);
        var color4 = new Color(0.700001f, 0.900001f, 1.000001f);

        // Test color addition
        var resultAdd = color1 + color2;
        Console.WriteLine($"Sum of ({color1.R}, {color1.G}, {color1.B}) and ({color2.R}, {color2.G}, {color2.B}) = ({resultAdd.R}, {Math.Round(resultAdd.G,4)}, {resultAdd.B})");  

        // Test color multiplication
        var resultMul = color1 * color2;
        Console.WriteLine($"Multiplication of ({color1.R}, {color1.G}, {color1.B}) and ({color2.R}, {color2.G}, {color2.B}) = ({resultMul.R}, {resultMul.G}, {resultMul.B})");

        // Test color multiplication with a scalar
        const float scalar = 2.0f;
        var resultMulScalar = color1 * scalar;
        Console.WriteLine($"Multiplication of ({color1.R}, {color1.G}, {color1.B}) with {scalar} = ({resultMulScalar.R}, {resultMulScalar.G}, {resultMulScalar.B})");

        // Test color equality
        var close1 = Color.are_close_colors(color1, color3);
        var close2 = Color.are_close_colors(color3, color4);
        Console.WriteLine($"The colors ({color1.R}, {color1.G}, {color1.B}) and ({color3.R}, {color3.G}, {color3.B}) are close: {close1}");
        Console.WriteLine($"The colors ({color3.R}, {color3.G}, {color3.B}) and ({color4.R}, {color4.G}, {color4.B}) are close: {close2}\n");
        
        //////// test HdrImage.cs /////////////////////
        
        // Create an HDR image with dimensions 3x3
        var image = new HdrImage(3, 3);

        // Test: Set the color of a pixel
        var newColor = new Color(1.0f, 0.0f, 0.0f); // Red
        image.SetPixel(1, 1, newColor);

        // Test: Retrieve the color of a pixel
        var pixelColor = image.GetPixel(1, 1);
        Console.WriteLine($"Pixel (1,1) color: ({pixelColor.R}, {pixelColor.G}, {pixelColor.B})");

        // Test: Verify that the color of another pixel is black (initial color)
        pixelColor = image.GetPixel(0, 0);
        Console.WriteLine($"Pixel (0,0) color: ({pixelColor.R}, {pixelColor.G}, {pixelColor.B})");
        
        // test to set pixel out of bounds
        image.SetPixel(2, 4, newColor);
        
        // test to get pixel out of bounds
        //var pix = image.GetPixel(4, 4);

    }
}
*/