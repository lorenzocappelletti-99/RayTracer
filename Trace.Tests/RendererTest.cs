/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using Xunit;
namespace Trace.Tests;



public class RendererTest
{
    private static readonly HdrImage Image = new HdrImage(width: 3, height: 3);
    private static readonly OrthogonalProjection? Camera = new OrthogonalProjection();
    private readonly ImageTracer _tracer = new ImageTracer(Image, Camera);
    
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
        
        var world = new World();
        world.AddShape(sphere);
        var renderer = new OnOffRenderer(world);
        _tracer.FireAllRays(world, renderer.Render);
        
        
        Assert.True(Image.GetPixel(0, 0).IsClose(Color.Black));
        Assert.True(Image.GetPixel(1, 0).IsClose(Color.Black));
        Assert.True(Image.GetPixel(2, 0).IsClose(Color.Black));
        Assert.True(Image.GetPixel(0, 1).IsClose(Color.Black));
        Assert.True(Image.GetPixel(1, 1).IsClose(Color.White));
        Assert.True(Image.GetPixel(2, 1).IsClose(Color.Black));
        Assert.True(Image.GetPixel(0, 2).IsClose(Color.Black));
        Assert.True(Image.GetPixel(1, 2).IsClose(Color.Black));
        Assert.True(Image.GetPixel(2, 2).IsClose(Color.Black));
    }

    [Fact]
    public void TestFlatRenderer()
    {
        var sphereColor = new Color(1.0f, 2.0f, 3.0f);
        var colMaterial = new Material
        {
            Pigment = new UniformPigment(new Color(1,2,3))  
        };
        var sphere = new Sphere(
            radius: 0.1f,
            transformation: Transformation.Translation(new Vec(2, 0, 0)) *
                            Transformation.Scaling(new Vec(.2f, .2f, .2f)),
            material: colMaterial);
        var world = new World();
        world.AddShape(sphere);
        var renderer = new FlatRenderer(world);
        _tracer.FireAllRays(world, renderer.Render);

        Assert.True(Image.GetPixel(0, 0).IsClose(Color.Black));
        Assert.True(Image.GetPixel(1, 0).IsClose(Color.Black));
        Assert.True(Image.GetPixel(2, 0).IsClose(Color.Black));
        Assert.True(Image.GetPixel(0, 1).IsClose(Color.Black));
        Assert.True(Image.GetPixel(2, 1).IsClose(Color.Black));
        Assert.True(Image.GetPixel(0, 2).IsClose(Color.Black));
        Assert.True(Image.GetPixel(1, 2).IsClose(Color.Black));
        Assert.True(Image.GetPixel(2, 2).IsClose(Color.Black));
        Assert.True(Image.GetPixel(1, 1).IsClose(sphereColor));
        
    }
}