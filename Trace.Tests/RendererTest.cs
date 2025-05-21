/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using Xunit;
namespace Trace.Tests;

public class RendererTest
{
    [Fact]
    public void TestOnOffRenderer()
    {
        var whiteMaterial = new Material
        {
            Pigment = new UniformPigment(Color.White)  
        };
        
        var sphere = new Sphere(
            radius: 0.1f,
            transformation: Transformation.Translation(new Vec(2, 0, 0)) *
                            Transformation.Scaling(new Vec(.2f, .2f, .2f)),
            material: whiteMaterial);
        var image = new HdrImage(width: 3, height: 3);
        var camera = new OrthogonalProjection();
        var tracer = new ImageTracer(image, camera);
        var world = new World();
        world.AddShape(sphere);
        var renderer = new OnOffRenderer(world);
        tracer.FireAllRays(world, renderer.Render);
        
        
        Assert.True(image.GetPixel(0, 0).IsClose(Color.Black));
        Assert.True(image.GetPixel(1, 0).IsClose(Color.Black));
        Assert.True(image.GetPixel(2, 0).IsClose(Color.Black));
        Assert.True(image.GetPixel(0, 1).IsClose(Color.Black));
        Assert.True(image.GetPixel(1, 1).IsClose(Color.White));
        Assert.True(image.GetPixel(2, 1).IsClose(Color.Black));
        Assert.True(image.GetPixel(0, 2).IsClose(Color.Black));
        Assert.True(image.GetPixel(1, 2).IsClose(Color.Black));
        Assert.True(image.GetPixel(2, 2).IsClose(Color.Black));
    }
}
/*
 class TestRenderers(unittest.TestCase):
    def testOnOffRenderer(self):
        sphere = Sphere(transformation=translation(Vec(2, 0, 0)) * scaling(Vec(0.2, 0.2, 0.2)),
                        material=Material(brdf=DiffuseBRDF(pigment=UniformPigment(WHITE))))
        image = HdrImage(width=3, height=3)
        camera = OrthogonalCamera()
        tracer = ImageTracer(image=image, camera=camera)
        world = World()
        world.add_shape(sphere)
        renderer = OnOffRenderer(world=world)
        tracer.fire_all_rays(renderer)

        assert image.get_pixel(0, 0).is_close(BLACK)
        assert image.get_pixel(1, 0).is_close(BLACK)
        assert image.get_pixel(2, 0).is_close(BLACK)

        assert image.get_pixel(0, 1).is_close(BLACK)
        assert image.get_pixel(1, 1).is_close(WHITE)
        assert image.get_pixel(2, 1).is_close(BLACK)

        assert image.get_pixel(0, 2).is_close(BLACK)
        assert image.get_pixel(1, 2).is_close(BLACK)
        assert image.get_pixel(2, 2).is_close(BLACK)

    def testFlatRenderer(self):
        sphere_color = Color(1.0, 2.0, 3.0)
        sphere = Sphere(transformation=translation(Vec(2, 0, 0)) * scaling(Vec(0.2, 0.2, 0.2)),
                        material=Material(brdf=DiffuseBRDF(pigment=UniformPigment(sphere_color))))
        image = HdrImage(width=3, height=3)
        camera = OrthogonalCamera()
        tracer = ImageTracer(image=image, camera=camera)
        world = World()
        world.add_shape(sphere)
        renderer = FlatRenderer(world=world)
        tracer.fire_all_rays(renderer)

        assert image.get_pixel(0, 0).is_close(BLACK)
        assert image.get_pixel(1, 0).is_close(BLACK)
        assert image.get_pixel(2, 0).is_close(BLACK)

        assert image.get_pixel(0, 1).is_close(BLACK)
        assert image.get_pixel(1, 1).is_close(sphere_color)
        assert image.get_pixel(2, 1).is_close(BLACK)

        assert image.get_pixel(0, 2).is_close(BLACK)
        assert image.get_pixel(1, 2).is_close(BLACK)
        assert image.get_pixel(2, 2).is_close(BLACK)

 */