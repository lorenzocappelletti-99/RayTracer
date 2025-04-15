namespace Trace;

public struct Ray
{
    public readonly Point Origin;
    public readonly Vec Direction;
    public readonly float Tmin = 1e-5f;
    public readonly float Tmax = float.PositiveInfinity;
    public readonly int Depth = 0;
    
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

    /// <summary>
    /// Check if two rays are similar enough to be considered equal
    /// </summary>
    /// <param name="b"></param>
    /// <param name="epsilon"></param>
    /// <returns></returns>
    public bool is_close(Ray b, float epsilon = 1e-5f)
    {
        return Point.AreClose(Origin, b.Origin) && Vec.AreClose(Direction, b.Direction);
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
    
    
/*
    /// <summary>
    ///
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public Ray Transform(Transformation transform)
    {
        return Ray(origin: transform * Origin, direction: transform * Direction);
    }
    */
    
}