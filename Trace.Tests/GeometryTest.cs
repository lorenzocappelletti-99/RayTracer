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
        
        Assert.True(a.IsClose(a));
        Assert.False(a.IsClose(b));
        Assert.True(a.IsClose(c));
    }

    [Fact]
    public void TestVectorOperations()
    {
        var a = new Vec(1.0f, 2.0f, 3.0f);
        var b = new Vec(4.0f, 6.0f, 8.0f);
        
        Assert.True( new Vec(-1.0f, -2.0f, -3.0f).IsClose(-a));
        Assert.True(new Vec(5.0f, 8.0f, 11.0f).IsClose(a + b));
        Assert.True(new Vec(3.0f, 4.0f, 5.0f).IsClose(b - a));
        Assert.True(new Vec(2.0f, 4.0f, 6.0f).IsClose(a * 2));
        Assert.True(new Vec(2.0f, 4.0f, 6.0f).IsClose(2 * a));
        Assert.True(new Vec(0.5f, 1.0f, 1.5f).IsClose(a / 2));
        
        Assert.Equal((double)40.0f, a * b, 5);
        Assert.True(new Vec(-2.0f, 4.0f, -2.0f).IsClose(a % b));
        Assert.True(new Vec(2.0f, -4.0f, 2.0f).IsClose(b % a));
        
        Assert.Equal((double)14.0f, a.SqNorm(), 5);
        Assert.Equal((double)14.0f, MathF.Pow(a.Norm(), 2), 5);
    }

    [Fact]
    private void TestOnb()
    {
        var pcg = new Pcg();
        for (var i = 0; i < 10000; i++)
        {
            var normal = new Vec(pcg.Random_float(), pcg.Random_float(), pcg.Random_float());
            normal.Normalize();
            var onb = Vec.CreateOnbFromZ(normal);
            
            Assert.True(onb.Item3.IsClose(normal));
            Assert.True(Misc.AreClose(onb.Item1 * onb.Item2, 0));
            Assert.True(Misc.AreClose(onb.Item2 * onb.Item3, 0));
            Assert.True(Misc.AreClose(onb.Item1 * onb.Item3, 0));
            
            Assert.True(Misc.AreClose(onb.Item1.Norm(), 1));
            Assert.True(Misc.AreClose(onb.Item2.Norm(), 1));
            Assert.True(Misc.AreClose(onb.Item3.Norm(), 1));
            
            Assert.True((onb.Item1 % onb.Item2).IsClose(onb.Item3));
            Assert.True((onb.Item2 % onb.Item3).IsClose(onb.Item1));
            Assert.True((onb.Item3 % onb.Item1).IsClose(onb.Item2));
        }
    }
    
    [Fact]
    public void TestPointOperations()
    {
        var a = new Point(1.0f, 2.0f, 3.0f);
        var b = new Point(4.0f, 6.0f, 8.0f);
        var vector = new Vec(4.0f, 6.0f, 8.0f);
        
        Assert.True(new Point(5.0f, 8.0f, 11.0f).IsClose(a + vector));
        Assert.True(new Point(-3.0f, -4.0f, -5.0f).IsClose(a - vector));
        Assert.True(new Vec(-3.0f, -4.0f, -5.0f).IsClose(a - b));
        Assert.True(new Vec(1.0f, 2.0f, 3.0f).IsClose(a.to_vec()));
    }
    
    
    [Fact]
    public void TestNormalOperations()
    {
        var a = new Normal(1.0f, 2.0f, 3.0f);
        var a1 = new Normal(4.0f, 6.0f, 8.0f);
        var b = new Vec(4.0f, 6.0f, 8.0f);
        
        Assert.True((a * 2.0f).IsClose(new Normal(2.0f, 4.0f, 6.0f)));
        Assert.True((2.0f * a).IsClose(new Normal(2.0f, 4.0f, 6.0f)));
        Assert.True((a / 2.0f).IsClose(new Normal(0.5f, 1.0f, 1.5f)));
        Assert.True((-a).IsClose(new Normal(-1.0f, -2.0f, -3.0f)));

        Assert.Equal((double)40.0f, b * a, 5);
        Assert.True(new Vec(-2.0f, 4.0f, -2.0f).IsClose(a % b));
        Assert.True(new Vec(-2.0f, 4.0f, -2.0f).IsClose(a % a1));
        
        Assert.True(new Vec(2.0f, -4.0f, 2.0f).IsClose(b % a));
        Assert.True(new Vec(2.0f, -4.0f, 2.0f).IsClose(a1 % a));
        
        Assert.Equal((double)14.0f, Normal.SqNorm(a), 5);
        Assert.Equal((double)14.0f, MathF.Pow(Normal.Norm(a), 2), 5);
    }
}