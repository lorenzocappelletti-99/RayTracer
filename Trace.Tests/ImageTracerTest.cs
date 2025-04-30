/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using Xunit;

namespace Trace.Tests;

public class ImageTracerTest
{
    [Fact]
    public void TestImageTracer()
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
}