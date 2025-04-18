namespace Trace;

public abstract class Camera
{
    public float AspectRatio { get; set; }
    public Transformation Transform { get; set; }
    
    public float Distance { get; set; }
    
    protected Camera(Transformation transform, float distance = 1.0f, float aspectRatio = 1.0f)
    {
        AspectRatio = aspectRatio;
        Distance = distance;
        Transform = transform;
    }

    
    protected Camera(float aspectRatio = 1.0f)
    {
        AspectRatio = aspectRatio;
    }
    
    public abstract Ray FireRay(float u, float v);
}

public class OrthogonalProjection : Camera
{
    /*
    public OrthogonalProjection(Transformation? transform = null, float aspectRatio = 1.0f)
        : base(transform, aspectRatio)
    {
    }
    */
    public OrthogonalProjection(float aspectRatio = 1.0f)
        : base(new Transformation(), 1.0f, aspectRatio)
    {
    }


    public override Ray FireRay(float u, float v)
    {
        var origin = new Point(-1.0f, (1.0f - 2.0f * u) * AspectRatio, 2.0f * v - 1.0f);
        var direction = Vec.VEC_X;
        return new Ray(origin, direction, 1.0e-5f).Transform(Transform);
    }
}

public class PerspectiveProjection : Camera
{

    public PerspectiveProjection(Transformation transform, float distance = 1.0f, float aspectRatio = 1.0f)
        : base(transform, distance, aspectRatio)
    {
    }
    public PerspectiveProjection(Transformation transform, float aspectRatio = 1.0f)
        : base(transform, aspectRatio)
    {
    }
    public PerspectiveProjection(float aspectRatio = 1.0f, float distance = 1.0f)
        : base(new Transformation(), distance, aspectRatio)
    
    {
    }
    
    /*    def __init__(self, distance=1.0, aspect_ratio=1.0, transformation=Transformation()):
        self.distance = distance
        self.aspect_ratio = aspect_ratio
        self.transformation = transformation

    def fire_ray(self, u: float, v: float) -> Ray:
        origin = Point(-self.distance, 0.0, 0.0)
        direction = Vec(self.distance, (1.0 - 2 * u) * self.aspect_ratio, 2 * v - 1)
        return Ray(origin=origin, dir=direction, tmin=1.0e-5).transform(self.transformation)*/

    public override Ray FireRay(float u, float v)
    {
        var origin = new Point(-Distance, 0.0f, 0.0f);
        var direction = new Vec(Distance, (1.0f - 2.0f * u) * AspectRatio, 2 * v - 1);
        return new Ray(origin, direction, 1.0e-5f).Transform(Transform);
    }
}