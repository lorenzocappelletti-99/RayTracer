namespace Trace;

public struct Ray
{
    public Point Origin;
    public Vec Direction;
    public float Tmin = 1e-5f;
    public float Tmax = float.PositiveInfinity;
    public const int Depth = 0;

    public Ray(Point origin, Vec direction, float tmin, float tmax, int depth)
    {
        Origin = origin;
        Direction = direction;
        Tmin = tmin;
        Tmax = tmax;
    }
    public Ray(Point origin, Vec direction)
    {
        Origin = origin;
        Direction = direction;
    }
    public Ray(Point origin, Vec direction, float tmin)
    {
        Origin = origin;
        Direction = direction;
        Tmin = tmin;
    }
    
    public override string ToString()
    {
        return $"Ray(Origin={Origin}, Direction={Direction}, " +
               $"Tmin={Tmin}, Tmax={Tmax}, Depth={Depth})";
    }

    /// <summary>
    /// Check if two rays are similar enough to be considered equal
    /// </summary>
    /// <param name="b"></param>
    /// <param name="epsilon"></param>
    /// <returns></returns>
    public bool is_close(Ray b, float epsilon = 1e-5f)
    {
        return Origin.IsClose(b.Origin) && Direction.IsClose(b.Direction);
    }
    
    /// <summary>
    /// Compute the point along the ray's path at some distance from the origin.
    /// Return a ``Point`` object representing the point in 3D space whose distance from the
    /// ray's origin is equal to `t`, measured in units of the length of `Vec.dir`.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Point PointAt(float t)
    {
        return Origin + Direction * t;
    }
    
    /// <summary>
    /// Applies a transformation to the ray, modifying both its origin and direction.
    /// This returns a new ray, transformed according to the given transformation matrix.
    /// </summary>
    /// <param name="transformation">The transformation to apply to the ray.</param>
    /// <returns>A new ray with the transformed origin and direction.</returns>
    public Ray Transform(Transformation transformation)
       {
           return new Ray(
               origin: transformation * Origin,
               direction: transformation * Direction,
               tmin: this.Tmin,
               tmax: this.Tmax,
               depth: Depth
           );
       }
   
    
}