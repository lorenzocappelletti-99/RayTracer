using Xunit;
namespace Trace.Tests;

public class RayTests
{
    [Fact]
    public void Test_at()
    {
        var ray = new Ray(origin: new Point(1.0f, 2.0f, 4.0f), direction: new Vec(4.0f, 2.0f, 1.0f));
        
        Assert.True(Point.AreClose(ray.PointAt(0.0f),ray.Origin));
        Assert.True(Point.AreClose(ray.PointAt(1.0f),new Point(5.0f, 4.0f, 5.0f)));
        Assert.True(Point.AreClose(ray.PointAt(2.0f) ,new Point(9.0f, 6.0f, 6.0f)));
    }
    
    [Fact]
    public void TestRays()
    {
        var ray1 = new Ray(    origin: new Point(1.0f, 2.0f, 3.0f), 
            direction: new Vec(5.0f, 4.0f, -1.0f));
        var ray2 = new Ray(    origin: new Point(1.0f, 2.0f, 3.0f), 
            direction: new Vec(5.0f, 4.0f, -1.0f));
        var ray3 = new Ray(    origin: new Point(5.0f, 1.0f, 4.0f), 
            direction: new Vec(3.0f, 9.0f, 4.0f));

        Assert.True(ray1.is_close(ray2));
        Assert.True(!ray1.is_close(ray3));
    }
}