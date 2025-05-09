/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using Xunit;
namespace Trace.Tests;

public class RayTests
{
    [Fact]
    public void Test_at()
    {
        var ray = new Ray(origin: new Point(1.0f, 2.0f, 4.0f), direction: new Vec(4.0f, 2.0f, 1.0f));
        
        Assert.True(ray.Origin.IsClose(ray.PointAt(0.0f)));
        Assert.True(new Point(5.0f, 4.0f, 5.0f).IsClose(ray.PointAt(1.0f)));
        Assert.True(new Point(9.0f, 6.0f, 6.0f).IsClose(ray.PointAt(2.0f) ));
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

        Assert.True(ray1.IsClose(ray2));
        Assert.True(!ray1.IsClose(ray3));
    }

    [Fact]
    public void Test_rayTransformation()
    {
        var ray = new Ray(origin: new Point(1.0f, 2.0f, 3.0f), direction: new Vec(6.0f, 5.0f, 4.0f));
        var trasl = Transformation.Translation(new Vec(10.0f, 11.0f, 12.0f));
        var rotx = Transformation.RotationX(90.0f);
        var tx = trasl*rotx;

        var transformed = ray.Transform(tx);

        Assert.True(transformed.Origin.IsClose(new Point(11.0f, 8.0f, 14.0f)));
        Assert.True(transformed.Direction.IsClose(new Vec(6.0f, -4.0f, 5.0f)));

    }
}