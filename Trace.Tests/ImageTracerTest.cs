/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/
using Xunit;

namespace Trace.Tests;

public class MyTests : IDisposable
{
    public ImageTracer Tracer;
    public HdrImage Image;

    public MyTests()
    {
        Image = new HdrImage(4, 2);
        var camera = new PerspectiveProjection(aspectRatio: 2.0f);
        Tracer = new ImageTracer(Image, camera);
    }
    
    
    [Fact]
    public void TestImageTracerRayCloseness()
    {
        // Arrange
        var ray1 = Tracer.FireRay(col: 0, row: 0, uPixel: 2.5f, vPixel: 1.5f);
        var ray2 = Tracer.FireRay(col: 2, row: 1, uPixel: 0.5f, vPixel: 0.5f);
        Assert.True(
            ray1.is_close(ray2),
            $"Expected ray1 and ray2 to be close, but got origins {ray1.Origin} vs {ray2.Origin}"
        );
    }
    [Fact]
    public void TestOrientation()
    {
        var topLeftRay = Tracer.FireRay(0, 0, uPixel: 0.0f, vPixel: 0.0f);
        var hotPointTop = new Point(0.0f, 2.0f, 2.0f);
        Assert.True( hotPointTop.IsClose(topLeftRay.PointAt(1.0f)) );
        
        var bottomLeftRay = Tracer.FireRay(3, 1, uPixel: 1.0f, vPixel: 1.0f);
        var hotPointBottom = new Point(0.0f, -2.0f, -1.0f);
        Assert.True( hotPointBottom.IsClose(bottomLeftRay.PointAt(1.0f)) );
    }
    [Fact]
    public void TestImageCoverage()
    {
        var color = new Color(1.0f, 2.0f, 3.0f);

        Tracer.FireAllRays(_ => color);

        // Assert: every pixel equals the fill color
        for (var row = 0; row < Image.Height; row++)
        {
            for (var col = 0; col < Image.Width; col++)
            {
                var pixel = Image.GetPixel(col, row);
                //testOutputHelper.WriteLine($"{pixel}");
                Assert.Equal(color, pixel);
            }
        }
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}


/*
public class ImageTracerTest
{
    [Fact]
    public void TestImageTracerRayCloseness()
    {
         // Arrange
         var image = new HdrImage(4,2);
         var camera = new PerspectiveProjection(aspectRatio: 2.0f);
         var tracer = new ImageTracer(image, camera);
         var ray1 = tracer.FireRay(col: 0, row: 0, uPixel: 2.5f, vPixel: 1.5f);
         var ray2 = tracer.FireRay(col: 2, row: 1, uPixel: 0.5f, vPixel: 0.5f);
         Assert.True(
             ray1.is_close(ray2),
             $"Expected ray1 and ray2 to be close, but got origins {ray1.Origin} vs {ray2.Origin}"
         );
    }

    [Fact]
    public void TestImageTracerRayCloseness2()
    {

        var color = new Color(1.0f, 2.0f, 3.0f);

        tracer.FireAllRays(ray => color);

        // Assert: every pixel equals the fill color
        for (var row = 0; row < image.Height; row++)
        {
            for (var col = 0; col < image.Width; col++)
            {
                var pixel = image.GetPixel(col, row);
                //testOutputHelper.WriteLine($"{pixel}");
                Assert.Equal(color, pixel);
            }
        }
    }
}*/