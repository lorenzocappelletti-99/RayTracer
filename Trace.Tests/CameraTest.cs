using Xunit;

namespace Trace.Tests;


public class CameraTest
{
    public static bool are_close(float a, float b, float epsilon = 1e-5f)
    {
        return Math.Abs(a - b) <= epsilon;
    }
    
    
    [Fact]
    public void TestCamera()
    {
        var cam = new OrthogonalProjection(aspectRatio:2.0f);
        
        var ray1 = cam.FireRay(0.0f, 0.0f);
        var ray2 = cam.FireRay(1.0f, 0.0f);
        var ray3 = cam.FireRay(0.0f, 1.0f);
        var ray4 = cam.FireRay(1.0f, 1.0f);

        Assert.True(are_close(0.0f, Vec.SqNorm(ray1.Direction%(ray2.Direction))));
        Assert.True(are_close(0.0f, Vec.SqNorm(ray1.Direction%(ray3.Direction))));
        Assert.True(are_close(0.0f, Vec.SqNorm(ray1.Direction%(ray4.Direction))));


        Assert.True(ray1.PointAt(1.0f).IsClose(new Point(0.0f, 2.0f, -1.0f)));
        Assert.True(ray2.PointAt(1.0f).IsClose(new Point(0.0f, -2.0f, -1.0f)));
        Assert.True(ray3.PointAt(1.0f).IsClose(new Point(0.0f, 2.0f, 1.0f)));
        Assert.True(ray4.PointAt(1.0f).IsClose(new Point(0.0f, -2.0f, 1.0f)));

        var camP = new PerspectiveProjection(1.0f, 1.0f);
        
        Assert.True(ray1.Origin.IsClose(ray2.Origin));
        Assert.True(ray1.Origin.IsClose(ray3.Origin));
        Assert.True(ray1.Origin.IsClose(ray4.Origin));
        


    }
    
}