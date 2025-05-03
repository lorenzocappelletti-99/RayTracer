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
        Ray ray1 = new Ray(origin: new Point(0, 0, 2), direction: -Vec.VEC_Z);
        Ray ray2 = new Ray(origin: new Point(3, 0, 0), direction: -Vec.VEC_X);
        
        Sphere sphere = new Sphere();

        HitRecord? hit1 = sphere.RayIntersection(ray1);
        HitRecord? hit2 = sphere.RayIntersection(ray2);
        
        Assert.NotNull(hit1);
        Assert.NotNull(hit2);

        var expected1 = new HitRecord
        {
            WorldPoint   = new Point(0f, 0f, 1f),
            Normal       = new Normal(0f, 0f, 1f),
            SurfacePoint = new Vec2d(0f, 0f),
            t            = 1f,
            Ray          = ray1
        };
        
        var expected2 = new HitRecord
        {
            WorldPoint   = new Point(1f, 0f, 0f),
            Normal       = new Normal(1f, 0f, 0f),
            SurfacePoint = new Vec2d(0f, 0.5f),
            t            = 2f,
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
        
        var expected = new HitRecord
        {
            WorldPoint   = new Point(1f, 0f, 0f),
            Normal       = new Normal(-1f, 0f, 0f),
            SurfacePoint = new Vec2d(0f, 0.5f),
            t            = 1f,
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
        
        var expected1 = new HitRecord
        {
            WorldPoint   = new Point(10f, 0f, 1f),
            Normal       = new Normal(0f, 0f, 1f),
            SurfacePoint = new Vec2d(0f, 0f),
            t            = 1f,
            Ray          = ray1
        };
        
        var expected2 = new HitRecord
        {
            WorldPoint   = new Point(11f, 0f, 0f),
            Normal       = new Normal(1f, 0f, 0f),
            SurfacePoint = new Vec2d(0f, 0.5f),
            t            = 2f,
            Ray          = ray2
        };
        
        Assert.True(expected1.IsClose(hit1));
        Assert.True(expected2.IsClose(hit2));
        
        Assert.Null(sphere.RayIntersection(new Ray(origin: new Point(0, 0, 2), direction: -Vec.VEC_Z)));
        Assert.Null(sphere.RayIntersection(new Ray(origin: new Point(-10, 0, 2), direction: -Vec.VEC_Z)));

    }
    
}