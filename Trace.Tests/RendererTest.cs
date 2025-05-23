/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using Xunit;
using Xunit.Abstractions;
namespace Trace.Tests;



public class RendererTest(ITestOutputHelper testOutputHelper)
{
    private static readonly HdrImage Image = new HdrImage(width: 3, height: 3);
    private static readonly OrthogonalProjection? Camera = new OrthogonalProjection();
    private readonly ImageTracer _tracer = new ImageTracer(Image, Camera);
    
    public static bool Approx(float a, float b, float epsilon = 1e-3f)
    {
        return MathF.Abs(a - b) < epsilon;
    }
    
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;
    [Fact]
    public void TestOnOffRenderer()
    {
        var whiteMaterial = new Material
        {
            EmittedRadiance = new UniformPigment(Color.White)  
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
            EmittedRadiance = new UniformPigment(new Color(1,2,3))  
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

    [Fact]
    public void TestPathTracer()
    {
        var pcg = new Pcg();

        for (int i = 0; i < 5; i++)
        {
            var scene = new World();

            var emittedRadiance = pcg.Random_float();
            var reflectance = pcg.Random_float() * 0.9f;
            
            _testOutputHelper.WriteLine($"emittedRadiance: {emittedRadiance}, reflectance: {reflectance}");
            
            var enclosureMaterial = new Material
            {
                Brdf = new DiffusiveBrdf {
                    Pigment = new UniformPigment(Color.White * reflectance) 
                },
                
                EmittedRadiance = new UniformPigment(Color.White * emittedRadiance)
            };
            
            scene.AddShape(new Sphere(
                material: enclosureMaterial)
            );

            var pathTracer = new PathTracer(
                world: scene, 
                pcg: pcg,
                numOfRays: 1, 
                maxDepth: 100,   
                russianRouletteLimit: 101 
            );
            
            var ray = new Ray(
                origin: new Point(0f, 0f, 0f), 
                direction: new Vec(1f, 0f, 0f)
                );

            var color = pathTracer.Render(ray);
            
            var expected = emittedRadiance / (1.0f - reflectance);
            _testOutputHelper.WriteLine($"expected: {expected}, actual: {color.R}, {color.G}, {color.B}");
            
            Assert.True(Approx(expected, color.R));
            Assert.True(Approx(expected, color.G));
            Assert.True(Approx(expected, color.B));
        }
    }

    [Fact]
    public void FurnaceTest()
    {
        var pcg = new Pcg();
        for (var i = 0; i < 5; i++)
        {
            var emittedRadiance = pcg.Random_float();
            var reflectance = pcg.Random_float();
            
            var world = new World();
            
            var enclosureMaterial = new Material
            {
                Brdf = new DiffusiveBrdf {
                    Pigment = new UniformPigment(Color.White * reflectance) 
                },
                EmittedRadiance = new UniformPigment(Color.White * emittedRadiance)
            };
            
            world.AddShape(new Sphere(material: enclosureMaterial));
            var pathTracer = new PathTracer(world: world, pcg: pcg, numOfRays: 1, maxDepth: 100, russianRouletteLimit: 101);

            var ray = new Ray(origin: new Point(0.0f, 0.0f, 0.0f), direction: new Vec(1f, 0f, 0f));
            var color = pathTracer.Render(ray);
            var expected = emittedRadiance / (1.0f - reflectance);
            _testOutputHelper.WriteLine($"expected: {expected}, actual: {color.R}, {color.G}, {color.B}");
            Assert.True(Approx(expected, color.R));
            Assert.True(Approx(expected, color.G));
            Assert.True(Approx(expected, color.B));
        }
    }
}