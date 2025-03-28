using Xunit;

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

        var result = color1 * color2;
        
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

    [Fact]
    public void TestLuminosity()
    {
        // Arrange
        var color1 = new Color(5.0f, 6.0f, 7.0f);
        var color2 = new Color(1.0f, 3.0f, 4.0f);
        float expectedLuminosity1 = (5+7)/2.0f;
        float expectedLuminosity2 = (1+4)/2.0f;

        // Act
        float actualLuminosity1 = color1.Luminosity();
        float actualLuminosity2 = color2.Luminosity();

        // Assert con epsilon
        float epsilon = 1e-5f;
        Assert.True(Math.Abs(expectedLuminosity1 - actualLuminosity1) < epsilon, 
            $"Expected {expectedLuminosity1}, but got {actualLuminosity1}");
        Assert.True(Math.Abs(expectedLuminosity2 - actualLuminosity2) < epsilon, 
            $"Expected {expectedLuminosity2}, but got {actualLuminosity2}");
        
    }

}