/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using System.Diagnostics;

namespace Trace;

public class ImageTracer
{
    public HdrImage Image { get; }
    public Camera? Camera { get; }
    public int SamplesPerSide;
    public Pcg Pcg;

    /// <summary>
    /// Constructs an ImageTracer using the specified HdrImage and camera.
    /// </summary>
    /// <param name="image">The HDR image to work on.</param>
    /// <param name="camera">The camera used to fire rays.</param>
    /// <param name="samplesPerSide"></param>
    /// <param name="pcg"></param>
    public ImageTracer(HdrImage image, Camera? camera, int samplesPerSide, Pcg pcg)
    {
        Image = image;
        Camera = camera;
        Pcg = pcg;
        SamplesPerSide = samplesPerSide;
    }
    
    public ImageTracer(HdrImage image, Camera? camera)
    {
        Image = image;
        Camera = camera;
        Pcg = new Pcg();
        SamplesPerSide = 0;
    }

    /// <summary>
    /// Fires a ray through the camera at the given pixel coordinates,
    /// using u_pixel and v_pixel as subpixel offsets.
    /// </summary>
    /// <param name="col">The column of the pixel.</param>
    /// <param name="row">The row of the pixel.</param>
    /// <param name="uPixel">The horizontal subpixel offset (default 0.5).</param>
    /// <param name="vPixel">The vertical subpixel offset (default 0.5).</param>
    /// <returns>A ray fired through the camera corresponding to the pixel.</returns>
    public Ray FireRay(int col, int row, float uPixel = 0.5f, float vPixel = 0.5f)
    {
        var u = (col + uPixel) / Image.Width;  
        var v = 1.0f - (row + vPixel) / Image.Height;
        Debug.Assert(Camera != null, nameof(Camera) + " != null");
        return Camera.FireRay(u, v);
    }
/*
    /// <summary>
    /// Fires a ray for each pixel in the image, applies the specified function(rendering) to compute a color,
    /// and sets the pixel in the image to the computed color.
    /// </summary>
    /// <param name="func">A function that, given a Ray, returns a Color (i.e. solving rendering equation).</param>
    public void FireAllRays(Func <Ray, Color> func)
    {
        for (var row = 0; row < Image.Height; row++)
        {
            for (var col = 0; col < Image.Width; col++)
            {
                var ray = FireRay(col, row);
                var color = func(ray);
                Image.SetPixel(col, row, color);
            }
        }
    }*/
    
    public void FireAllRays(World? scene, Func <Ray, Color> func)
    {
        if (scene == null)
        {
            Image.SetAllPixels(Color.Black);
            return;
        }

        for (var row = 0; row < Image.Height; row++)
        {
            for (var col = 0; col < Image.Width; col++)
            {
                var cumColor = new Color(0, 0, 0);

                if (SamplesPerSide > 0)
                {
                    for (var interPixelRow = 0; interPixelRow < SamplesPerSide; interPixelRow++)
                    {
                        for (var interPixelCol = 0; interPixelCol < SamplesPerSide; interPixelCol++)
                        {
                            var uPixel = (interPixelCol + Pcg.Random_float())/SamplesPerSide;
                            var vPixel = (interPixelRow + Pcg.Random_float())/SamplesPerSide;
                            var ray = FireRay(col, row, uPixel, vPixel);
                            cumColor += func(ray);
                        }
                    }
                    Image.SetPixel(col, row, cumColor * (1.0f/(SamplesPerSide*SamplesPerSide)));
                }
                else
                {
                    var ray = FireRay(col, row);
                    Image.SetPixel(col, row, func(ray));
                }
            }
        }
    }
}