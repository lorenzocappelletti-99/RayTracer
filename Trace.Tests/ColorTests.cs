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
}