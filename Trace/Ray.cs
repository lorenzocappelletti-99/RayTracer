/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

namespace Trace;

public struct Ray
{
    public Point Origin;
    public Vec Direction;
    public float Tmin;
    public float Tmax;
    public int Depth;

    public Ray(Point origin, Vec direction, float tmin = 1e-5f, float tmax = float.PositiveInfinity, int depth = 0)
    {
        Origin = origin;
        Direction = direction;
        Tmin = tmin;
        Tmax = tmax;
        Depth = depth;
    }
    
    
    /// <summary>
    /// Returns the description of ray parameters as a string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"Ray(Origin={Origin}, Direction={Direction}, " +
               $"Tmin={Tmin}, Tmax={Tmax}, Depth={Depth})";
    }

    /// <summary>
    /// Check if two rays are similar enough to be considered equal
    /// </summary>
    /// <param name="other"></param>
    /// <param name="epsilon"></param>
    /// <returns></returns>
    public bool IsClose(Ray other, float epsilon = 1e-5f)
    {
        return Origin.IsClose(other.Origin) && Direction.IsClose(other.Direction);
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