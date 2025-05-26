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
    ///  Construct an ImageTracer object
    /// </summary>
    /// <param name="image">must be a `HdrImage` object that has already been initialized</param>
    /// <param name="camera">must be a descendant of the `Camera` object.</param>
    /// <param name="samplesPerSide">If `samples_per_side` is larger than zero, stratified sampling will be applied to each pixel in the image, using the random number generator `pcg`.</param>
    /// <param name="pcg"></param>
    public ImageTracer(HdrImage image, Camera? camera, int samplesPerSide, Pcg pcg)
    {
        Image = image;
        Camera = camera;
        SamplesPerSide = samplesPerSide;
        Pcg = pcg;
    }
    

    public ImageTracer(HdrImage image, Camera? camera)
    {
        Image = image;
        Camera = camera;
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
    
    public void FireAllRays(Func <Ray, Color> func)
    {
        /*
        if (scene == null)
        {
            Image.SetAllPixels(Color.Black);
            return;
        }
        */
        
        for (var row = 0; row < Image.Height; row++)
        {
            for (var col = 0; col < Image.Width; col++)
            {
                var cumColor = new Color(0, 0, 0);

                if (SamplesPerSide > 0)
                {
                    for (var intPixelRow = 0; intPixelRow < SamplesPerSide; intPixelRow++)
                    {
                        for (var intPixelCol = 0; intPixelCol < SamplesPerSide; intPixelCol++)
                        {
                            var uPixel = (intPixelCol + Pcg.Random_float());
                            var vPixel = (intPixelRow + Pcg.Random_float()); 
                            var ray = FireRay(col, row, uPixel, vPixel);
                            cumColor += func(ray);
                        }
                    }
                    Image.SetPixel(col, row, cumColor* (1.0f/(SamplesPerSide*SamplesPerSide)));
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