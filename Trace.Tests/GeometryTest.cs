using Xunit;

namespace Trace.Tests;

public class GeometryTests
{
    [Fact]
    public void TestVectors()
    {
        var a = new Vec(1.0f, 2.0f, 3.0f);
        var b = new Vec(4.0f, 6.0f, 8.0f);
        var c = new Vec(1.000001f, 2.000001f, 3.000001f);
        
        Assert.True(Vec.AreClose(a, a));
        Assert.False(Vec.AreClose(a, b));
        Assert.True(Vec.AreClose(a, c));
    }

    [Fact]
    public void TestVectorOperations()
    {
        var a = new Vec(1.0f, 2.0f, 3.0f);
        var b = new Vec(4.0f, 6.0f, 8.0f);
        
        Assert.True(Vec.AreClose(-a, new Vec(-1.0f, -2.0f, -3.0f)));
        Assert.True(Vec.AreClose(a + b, new Vec(5.0f, 8.0f, 11.0f)));
        Assert.True(Vec.AreClose(b - a, new Vec(3.0f, 4.0f, 5.0f)));
        Assert.True(Vec.AreClose(a * 2, new Vec(2.0f, 4.0f, 6.0f)));
        
        Assert.Equal((double)40.0f, a * b, 5);
        Assert.True(Vec.AreClose(a % b, new Vec(-2.0f, 4.0f, -2.0f)));
        Assert.True(Vec.AreClose(b % a, new Vec(2.0f, -4.0f, 2.0f)));
        
        Assert.Equal((double)14.0f, Vec.SqNorm(a), 5);
        Assert.Equal((double)14.0f, MathF.Pow(Vec.Norm(a), 2), 5);
    }
}