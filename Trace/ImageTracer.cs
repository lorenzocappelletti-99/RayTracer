namespace Trace;

public class ImageTracer
{
    public HdrImage Image { get; }
    public ICamera Camera { get; }

    /// <summary>
    /// Constructs an ImageTracer using the specified HdrImage and camera.
    /// </summary>
    /// <param name="image">The HDR image to work on.</param>
    /// <param name="camera">The camera used to fire rays.</param>
    public ImageTracer(HdrImage image, ICamera camera)
    {
        Image = image;
        Camera = camera;
    }

    /// <summary>
    /// Fires a ray through the camera at the given pixel coordinates,
    /// using u_pixel and v_pixel as subpixel offsets.
    /// </summary>
    /// <param name="col">The column of the pixel.</param>
    /// <param name="row">The row of the pixel.</param>
    /// <param name="u_pixel">The horizontal subpixel offset (default 0.5).</param>
    /// <param name="v_pixel">The vertical subpixel offset (default 0.5).</param>
    /// <returns>A ray fired through the camera corresponding to the pixel.</returns>
    public Ray FireRay(int col, int row, float u_pixel = 0.5f, float v_pixel = 0.5f)
    {
        var u = (col + u_pixel) / (Image.Width - 1);
        var v = (row + v_pixel) / (Image.Height - 1);
        return Camera.FireRay(u, v);
    }

    /// <summary>
    /// Fires a ray for each pixel in the image, applies the specified function(rendering) to compute a color,
    /// and sets the pixel in the image to the computed color.
    /// </summary>
    /// <param name="func">A function that, given a Ray, returns a Color (i.e. solving rendering equation).</param>
    public void FireAllRays(Func <Ray, Color> func)
    {
        for (int row = 0; row < Image.Height; row++)
        {
            for (int col = 0; col < Image.Width; col++)
            {
                Ray ray = FireRay(col, row);
                Color color = func(ray);
                Image.SetPixel(col, row, color);
            }
        }
    }
}