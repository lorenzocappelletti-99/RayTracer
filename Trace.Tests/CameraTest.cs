/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using Xunit;

namespace Trace.Tests;


public class CameraImageTracerTest
{
    private static bool are_close(float a, float b, float epsilon = 1e-5f)
    {
        return Math.Abs(a - b) <= epsilon;
    }


    [Fact]
    public void TestOrthogonalCamera()
    {
        var cam = new OrthogonalProjection(2.0f);

        var ray1 = cam.FireRay(0.0f, 0.0f);
        var ray2 = cam.FireRay(1.0f, 0.0f);
        var ray3 = cam.FireRay(0.0f, 1.0f);
        var ray4 = cam.FireRay(1.0f, 1.0f);

        Assert.True(are_close(0.0f, (ray1.Direction % ray3.Direction).SqNorm()));
        Assert.True(are_close(0.0f, (ray1.Direction % ray2.Direction).SqNorm()));
        Assert.True(are_close(0.0f, (ray1.Direction % ray4.Direction).SqNorm()));


        //testOutputHelper.WriteLine($"{ray1}");

        Assert.True(ray1.PointAt(1.0f).IsClose(new Point(0.0f, 2.0f, -1.0f)));
        Assert.True(ray2.PointAt(1.0f).IsClose(new Point(0.0f, -2.0f, -1.0f)));
        Assert.True(ray3.PointAt(1.0f).IsClose(new Point(0.0f, 2.0f, 1.0f)));
        Assert.True(ray4.PointAt(1.0f).IsClose(new Point(0.0f, -2.0f, 1.0f)));

    }

    [Fact]
    public void TestOrthogonalCameraTransform()
    {
        var rotZ = Transformation.RotationZ(90f);
        var trasl = Transformation.Translation(new Vec(0f, -2f, 0f));
        var combined = trasl * rotZ;

        var cam = new OrthogonalProjection(
            aspectRatio: 1.0f,
            transform: combined
        );

        var ray = cam.FireRay(0.5f, 0.5f);
        var pointAt1 = ray.PointAt(1.0f);

        var expected = new Point(0f, -2f, 0f);
        Assert.True(expected.IsClose(pointAt1), $"Expected point {expected}, but got {pointAt1}"
        );
    }


    [Fact]
    public void TestPerspectiveCamera()
    {
        var cam = new PerspectiveProjection(
            aspectRatio: 2.0f,
            distance: 1.0f
        );

        var ray1 = cam.FireRay(0.0f, 0.0f);
        var ray2 = cam.FireRay(1.0f, 0.0f);
        var ray3 = cam.FireRay(0.0f, 1.0f);
        var ray4 = cam.FireRay(1.0f, 1.0f);

        Assert.True(ray1.Origin.IsClose(ray2.Origin));
        Assert.True(ray1.Origin.IsClose(ray3.Origin));
        Assert.True(ray1.Origin.IsClose(ray4.Origin));

        // Assert: corners hit at t=1 have correct coordinates
        Assert.True(ray1.PointAt(1.0f).IsClose(new Point(0f, 2f, -1f)));
        Assert.True(ray2.PointAt(1.0f).IsClose(new Point(0f, -2f, -1f)));
        Assert.True(ray3.PointAt(1.0f).IsClose(new Point(0f, 2f, 1f)));
        Assert.True(ray4.PointAt(1.0f).IsClose(new Point(0f, -2f, 1f)));

    }
}