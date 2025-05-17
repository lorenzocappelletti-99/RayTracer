/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using Xunit;
namespace Trace.Tests;

public class ShapeTests
{
    [Fact]
    public void TestSphereOuter()
    {
        var ray1 = new Ray(origin: new Point(0, 0, 2), direction: -Vec.VEC_Z);
        var ray2 = new Ray(origin: new Point(3, 0, 0), direction: -Vec.VEC_X);
        
        Sphere sphere = new Sphere();

        HitRecord? hit1 = sphere.RayIntersection(ray1);
        HitRecord? hit2 = sphere.RayIntersection(ray2);
        
        Assert.NotNull(hit1);
        Assert.NotNull(hit2);
        
        Assert.True(sphere.QuickRayIntersection(ray1));
        Assert.True(sphere.QuickRayIntersection(ray2));

        var expected1 = new HitRecord
        {
            WorldPoint   = new Point(0f, 0f, 1f),
            Normal       = new Normal(0f, 0f, 1f),
            SurfacePoint = new Vec2d(0f, 0f),
            T            = 1f,
            Ray          = ray1
            
        };
        
        var expected2 = new HitRecord
        {
            WorldPoint   = new Point(1f, 0f, 0f),
            Normal       = new Normal(1f, 0f, 0f),
            SurfacePoint = new Vec2d(0f, 0.5f),
            T            = 2f,
            Ray          = ray2
        };
        
        Assert.True(expected1.IsClose(hit1));
        Assert.True(expected2.IsClose(hit2));
    }

    [Fact]
    public void TestSphereInner()
    {
        var sphere = new Sphere();
        
        Ray ray = new Ray(origin: new Point(0, 0, 0), direction: Vec.VEC_X);
        
        var hit = sphere.RayIntersection(ray);
        
        Assert.NotNull(hit);
        Assert.True(sphere.QuickRayIntersection(ray));
        
        var expected = new HitRecord
        {
            WorldPoint   = new Point(1f, 0f, 0f),
            Normal       = new Normal(-1f, 0f, 0f),
            SurfacePoint = new Vec2d(0f, 0.5f),
            T            = 1f,
            Ray          = ray
        };

        Assert.True(expected.IsClose(hit));
    }
    

    [Fact]
    public void TestSphereTransformation()
    {
        var translation = Transformation.Translation(new Vec(10, 0, 0));

        var sphere = new Sphere(transformation : translation);
        
        Ray ray1 = new Ray(origin: new Point(10, 0, 2), direction: -Vec.VEC_Z);
        Ray ray2 = new Ray(origin: new Point(13, 0, 0), direction: -Vec.VEC_X);
        
        HitRecord? hit1 = sphere.RayIntersection(ray1);
        HitRecord? hit2 = sphere.RayIntersection(ray2);
        
        Assert.NotNull(hit1);
        Assert.NotNull(hit2);
        Assert.True(sphere.QuickRayIntersection(ray1));
        Assert.True(sphere.QuickRayIntersection(ray2));
        
        var expected1 = new HitRecord
        {
            WorldPoint   = new Point(10f, 0f, 1f),
            Normal       = new Normal(0f, 0f, 1f),
            SurfacePoint = new Vec2d(0f, 0f),
            T            = 1f,
            Ray          = ray1
        };
        
        var expected2 = new HitRecord
        {
            WorldPoint   = new Point(11f, 0f, 0f),
            Normal       = new Normal(1f, 0f, 0f),
            SurfacePoint = new Vec2d(0f, 0.5f),
            T            = 2f,
            Ray          = ray2
        };
        
        Assert.True(expected1.IsClose(hit1));
        Assert.True(expected2.IsClose(hit2));
        
        Assert.Null(sphere.RayIntersection(new Ray(origin: new Point(0, 0, 2), direction: -Vec.VEC_Z)));
        Assert.Null(sphere.RayIntersection(new Ray(origin: new Point(-10, 0, 2), direction: -Vec.VEC_Z)));

        Assert.False(sphere.QuickRayIntersection(new Ray(origin: new Point(0, 0, 2), direction: -Vec.VEC_Z)));
        Assert.False(sphere.QuickRayIntersection(new Ray(origin: new Point(-10, 0, 2), direction: -Vec.VEC_Z)));

    }

    [Fact]
    public void TestPlaneHit()
    {
        var plane = new Plane();

        var ray1 = new Ray(origin: new Point(0.0f, 0.0f, 1.0f), direction: -Vec.VEC_Z);
        var intersection1 = plane.RayIntersection(ray1);

        Assert.True(
            new HitRecord(
                worldPoint: new Point(0.0f, 0.0f, 0.0f), normal: 
                new Normal(0.0f, 0.0f, 1.0f),
                surfacePoint: new Vec2d(0.0f, 0.0f),
                t: 1.0f,
                ray: ray1).IsClose(intersection1));

        var ray2 = new Ray(origin: new Point(0, 0, 1), direction : Vec.VEC_Z);
        var intersection2 = plane.RayIntersection(ray2);
        Assert.Null(intersection2);
        Assert.False(plane.QuickRayIntersection(ray2));

        var ray3 = new Ray(origin : new Point(0, 0, 1), direction : Vec.VEC_X);
        var intersection3 = plane.RayIntersection(ray3);
        Assert.Null(intersection3);
        Assert.False(plane.QuickRayIntersection(ray3));

        var ray4 = new Ray(origin : new Point(0, 0, 1), direction: Vec.VEC_Y);
        var intersection4 = plane.RayIntersection(ray4);
        Assert.Null(intersection4);
        Assert.False(plane.QuickRayIntersection(ray4));

    }

    [Fact]
    public void TestPlaneHitTransform()
    {
        var plane = new Plane(transformation: Transformation.RotationY(angleDeg: 90.0f));
        
        var ray1 = new Ray(origin: new Point(1, 0, 0), direction: -Vec.VEC_X);
        Assert.NotNull(plane.RayIntersection(ray1));
        Assert.True(plane.QuickRayIntersection(ray1));
        
        var intersection1 = plane.RayIntersection(ray1);
        Assert.True(
            new HitRecord(
                worldPoint: new Point(0.0f, 0.0f, 0.0f), normal: 
                new Normal(1.0f, 0.0f, 0.0f),
                surfacePoint: new Vec2d(0.0f, 0.0f),
                t: 1.0f,
                ray: ray1).IsClose(intersection1));
        
        var ray2 = new Ray(origin: new Point(0, 0, 1), direction: Vec.VEC_Z);
        var intersection2 = plane.RayIntersection(ray2);
        Assert.Null(intersection2);
        Assert.False(plane.QuickRayIntersection(ray2));
        
        var ray3 = new Ray(origin : new Point(0, 0, 1), direction: Vec.VEC_X);
        var intersection3 = plane.RayIntersection(ray3);
        Assert.Null(intersection3);
        Assert.False(plane.QuickRayIntersection(ray3));
        
        var ray4 = new Ray(origin : new Point(0, 0, 1), direction: Vec.VEC_Y);
        var intersection4 = plane.RayIntersection(ray4);
        Assert.Null(intersection4);
        Assert.False(plane.QuickRayIntersection(ray4));
    }

    [Fact]
    public void TestUVCoordinates()
    {
        var plane = new Plane();
        
        var ray1 = new Ray(origin: new Point(0, 0, 1), direction: -Vec.VEC_Z);
        var intersection1 = plane.RayIntersection(ray1);
        Assert.True(intersection1?.SurfacePoint.IsClose(new Vec2d(0.0f, 0.0f)));

        var ray2 = new Ray(origin: new Point(0.25f, 0.75f, 1), direction: -Vec.VEC_Z);
        var intersection2 = plane.RayIntersection(ray2);
        Assert.True(intersection2?.SurfacePoint.IsClose(new Vec2d(0.25f, 0.75f)));

        var ray3 = new Ray(origin: new Point(4.25f, 7.75f, 1), direction:-Vec.VEC_Z);
        var intersection3 = plane.RayIntersection(ray3);
        Assert.True(intersection3?.SurfacePoint.IsClose(new Vec2d(0.25f, 0.75f)));
    }
}