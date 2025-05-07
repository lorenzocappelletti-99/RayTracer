/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using System.Runtime.CompilerServices;
using Xunit;
namespace Trace.Tests;

public class WorldTests
{
    [Fact]
    public void TestRayIntersection()
    {
        var world = new World();

        var sphere1 = new Sphere(transformation : Transformation.Translation(Vec.VEC_X * 2));
        var sphere2 = new Sphere(transformation : Transformation.Translation(Vec.VEC_X * 8));
        world.AddShape(sphere1);
        world.AddShape(sphere2);

        var intersection1 = world.ray_intersection(
            new Ray(
                origin:new Point(0.0f, 0.0f, 0.0f), 
                direction : Vec.VEC_X));
        Assert.NotNull(intersection1);
        Assert.True(intersection1.WorldPoint.IsClose(new Point(1,0,0)));

        var intersection2 = world.ray_intersection(
            new Ray(origin: new Point(10.0f, 0.0f, 0.0f), direction: -Vec.VEC_X
            ));
        Assert.NotNull(intersection2);
        Assert.True(intersection2.WorldPoint.IsClose(new Point(9,0,0)));
    }

    [Fact]
    public void TestIsPointVisible()
    {
        var world = new World();
        
        var sphere1 = new Sphere(transformation : Transformation.Translation(Vec.VEC_X * 2));
        var sphere2 = new Sphere(transformation : Transformation.Translation(Vec.VEC_X * 8));
        world.AddShape(sphere1);
        world.AddShape(sphere2);
        

    }
/**/
}
/*
        assert not world.is_point_visible(point=Point(10.0, 0.0, 0.0),
                                          observer_pos=Point(0.0, 0.0, 0.0))
        assert not world.is_point_visible(point=Point(5.0, 0.0, 0.0),
                                          observer_pos=Point(0.0, 0.0, 0.0))
        assert world.is_point_visible(point=Point(5.0, 0.0, 0.0),
                                      observer_pos=Point(4.0, 0.0, 0.0))
        assert world.is_point_visible(point=Point(0.5, 0.0, 0.0),
                                      observer_pos=Point(0.0, 0.0, 0.0))
        assert world.is_point_visible(point=Point(0.0, 10.0, 0.0),
                                      observer_pos=Point(0.0, 0.0, 0.0))
        assert world.is_point_visible(point=Point(0.0, 0.0, 10.0),
                                      observer_pos=Point(0.0, 0.0, 0.0))

*/