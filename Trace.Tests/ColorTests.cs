/* 
using System.Reflection.Metadata;

namespace Trace;

class Program
{
    static void Main(string[] args)
    {

        // Crea alcuni oggetti di colore per il test
        Color color1 = new Color(0.5f, 0.6f, 0.7f);
        Color color2 = new Color(0.2f, 0.3f, 0.4f);
        Color color3 = new Color(0.7f, 0.9f, 1.0f);

        // Test somma di colori
        Color resultAdd = color1 + color2;
        Console.WriteLine($"Somma di {color1.R}, {color1.G}, {color1.B} e {color2.R}, {color2.G}, {color2.B} = {resultAdd.R}, {resultAdd.G}, {resultAdd.B}");

        // Test moltiplicazione di colori
        Color resultMul = color1 * color2;
        Console.WriteLine($"Moltiplicazione di {color1.R}, {color1.G}, {color1.B} e {color2.R}, {color2.G}, {color2.B} = {resultMul.R}, {resultMul.G}, {resultMul.B}");

        // Test moltiplicazione di colore con scalare
        float scalar = 2.0f;
        Color resultMulScalar = color1 * scalar;
        Console.WriteLine($"Moltiplicazione di {color1.R}, {color1.G}, {color1.B} con {scalar} = {resultMulScalar.R}, {resultMulScalar.G}, {resultMulScalar.B}");

        // Test uguaglianza di colori con un epsilon
        bool close = Color.are_close_colors(color1, color3);
        Console.WriteLine($"I colori {color1.R}, {color1.G}, {color1.B} e {color3.R}, {color3.G}, {color3.B} sono vicini: {close}");
    }
}


namespace Trace.Tests;

public class ColorTests
{
    [Fact]
    public void TestColorAddition()
    {
        var color1 = new Color(5.0f, 6.0f, 7.0f);
        var color2 = new Color(2.0f, 3.0f, 4.0f);
        var expected = new Color(7.0f, 9.0f, 11.0f);

        var result = color1 + color2;
        
        Assert.True(Color.are_close_colors(expected, result));
    }

    [Fact]
    public void TestColorMultiplication()
    {
        var color1 = new Color(5.0f, 6.0f, 7.0f);
        var color2 = new Color(2.0f, 3.0f, 4.0f);
        var expected = new Color(10.0f, 18.0f, 28.0f);

        var result = color1 + color2;
        
        Assert.True(Color.are_close_colors(expected, result));
    }

    [Fact]
    public void TestColorScalarMultiplication()
    {
        var color1 = new Color(5.0f, 6.0f, 7.0f);
        const float scalar = 2.0f;
        var expected = new Color(10.0f, 12.0f, 14.0f);

        var result = scalar*color1;
        
        Assert.True(Color.are_close_colors(expected, result));
    }

    [Fact]
    public void TestColorCloseness()
    {
        var color1 = new Color(5.0f, 6.0f, 7.0f);
        var color3 = new Color(5.0f, 6.0f, 7.0f);
        var result = Color.are_close_colors(color1, color3);

        Assert.True(result);
    }
}
*/
using System;
using Xunit;
using Trace;

namespace Trace.Tests
{
    public class ColorTests
    {
        [Fact]
        public void TestAdd()
        {
            Color c1 = new Color(1.0f, 2.0f, 3.0f);
            Color c2 = new Color(5.0f, 6.0f, 7.0f);
            // C# convention: *first* the expected value, *then* the test value
            Assert.True(Color.are_close_colors(new Color(6.0f, 8.0f, 10.0f), c1 + c2));
            // ...
        }
    }
}



