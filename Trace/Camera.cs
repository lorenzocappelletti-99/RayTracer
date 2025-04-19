namespace Trace;

public abstract class Camera
{
    public float AspectRatio { get; set; }
    public Transformation Transform { get; set; }
    public float Distance { get; set; }

    // Tutti i parametri opzionali, con defaults:
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
public class OrthogonalProjection : Camera
{
    /// <summary>
    /// aspectRatio: width/height del film plane
    /// transform:  matrice di posizionamento/orientamento in world
    /// </summary>
    public OrthogonalProjection(
        float aspectRatio = 1.0f,
        Transformation? transform   = null
    ) : base(aspectRatio, distance: 1.0f, transform) { }

    public override Ray FireRay(float u, float v)
    {
        var origin    = new Point(
            -1.0f,
            (1.0f - 2.0f * u) * AspectRatio,
            2.0f * v - 1.0f
        );
        var direction = Vec.VEC_X;
        return new Ray(origin, direction, 1e-5f)
            .Transform(Transform);
    }
}

public class PerspectiveProjection : Camera
{
    /// <summary>
    /// aspectRatio: width/height del film plane
    /// distance:    distanza della camera dal film plane
    /// transform:   matrice di posizionamento/orientamento in world
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
        return new Ray(origin, direction, 1e-5f)
            .Transform(Transform);
    }
}
