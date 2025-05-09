/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

namespace Trace;


/// <summary>
/// Base Class
/// </summary>
public abstract class Camera
{
    public float AspectRatio { get; set; }
    public Transformation Transform { get; set; }
    public float Distance { get; set; }

    /// <summary>
    /// All parameters are optional. 
    /// </summary>
    /// <param name="aspectRatio"></param>
    /// <param name="distance"></param>
    /// <param name="transform"></param>
    protected Camera(
        float aspectRatio = 1.0f,
        float distance    = 1.0f,
        Transformation? transform = null
    )
    {
        AspectRatio = aspectRatio;
        Distance    = distance;
        Transform   = transform ?? new Transformation();
    }

    public abstract Ray FireRay(float u, float v);
}

/// <summary>
/// A camera implementing an orthogonal 3D → 2D projection
/// This class implements an observer seeing the world through an orthogonal projection.
/// </summary>
public class OrthogonalProjection : Camera
{
    /// <summary>
    /// aspectRatio: width/height of the film plane
    /// transform:  posizionamento/orientamento matrix in world
    /// distance : distance of the observer
    /// </summary>
    /// Create a new orthographic camera
    /// The parameter `aspect_ratio` defines how larger than the height is the image. For fullscreen
    /// images, you should probably set `aspect_ratio` to 16/9, as this is the most used aspect ratio
    /// used in modern monitors.
    /// The `transformation` parameter is an instance of the :class:`.Transformation` class.
    public OrthogonalProjection(
        float aspectRatio = 16/9f,
        Transformation? transform = null,
        float distance = 1.0f
    ) : base(aspectRatio, distance, transform) { }


    public override Ray FireRay(float u, float v)
    {
        var origin    = new Point(
            -1.0f,
            (1.0f - 2.0f * u) * AspectRatio,
            2.0f * v - 1.0f
        );
        var direction = Vec.VEC_X;
        return new Ray(origin, direction)
            .Transform(Transform);
    }
}

public class PerspectiveProjection : Camera
{
    /// <summary>
    /// aspectRatio: width/height of the film plane
    /// distance:    distance of camera from film plane
    /// transform:   posizionamento/orientamento matrix in world
    /// </summary>
    public PerspectiveProjection(
        float aspectRatio = 1.0f,
        float distance    = 1.0f,
        Transformation? transform   = null
    ) : base(aspectRatio, distance, transform) { }

    public override Ray FireRay(float u, float v)
    {
        var origin    = new Point(-Distance, 0.0f, 0.0f);
        var direction = new Vec(
            Distance,
            (1.0f - 2.0f * u) * AspectRatio,
            2.0f * v - 1.0f
        );
        return new Ray(origin, direction)
            .Transform(Transform);
    }
}
