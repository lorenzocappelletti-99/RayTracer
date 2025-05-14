/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

namespace Trace;

/// <summary>
/// Represents the result of a ray intersection with a shape, containing:
/// - <see cref="WorldPoint"/>: the hit position in world coordinates
/// - <see cref="Normal"/>: the surface normal at the hit point in world space
/// - <see cref="SurfacePoint"/>: the UV texture coordinates at the hit point
/// - <see cref="T"/>: the parameter along the ray where the hit occurred
/// - <see cref="Ray"/>: the original ray that produced this hit
/// </summary>
public class HitRecord
{

    public Point WorldPoint;
    public Normal Normal;
    public Vec2d SurfacePoint;
    public float T;
    public Ray   Ray;
    public Material Material;

    public HitRecord(){}

    public HitRecord(
        Point worldPoint, 
        Normal normal, 
        Vec2d surfacePoint,
        float t,
        Ray ray)
    {
        WorldPoint = worldPoint;
        Normal = normal;
        SurfacePoint = surfacePoint;
        this.T = t;
        Ray = ray;
    }
    
    public HitRecord(Point worldPoint, 
        Normal normal, 
        Vec2d surfacePoint,
        float t,
        Ray ray,
        Material material)
    {
        WorldPoint = worldPoint;
        Normal = normal;
        SurfacePoint = surfacePoint;
        this.T = t;
        Ray = ray;
        Material = material;
    }
    
    
    /// <summary>
    /// Checks whether this hit record and another are approximately equal.
    /// </summary>
    /// <param name="other">The other HitRecord to compare against.</param>
    /// <param name="epsilon">Tolerance for floating-point comparisons.</param>
    /// <returns>True if all components are within epsilon, false otherwise.</returns>
    public bool IsClose(HitRecord? other, float epsilon = 1e-5f)
    {
        if (other == null)
            return false;

        return
            WorldPoint.IsClose(other.WorldPoint, epsilon) &&
            Normal.IsClose(other.Normal, epsilon) &&
            SurfacePoint.IsClose(other.SurfacePoint, epsilon) &&
            Math.Abs(T - other.T) < epsilon &&
            Ray.IsClose(other.Ray, epsilon);
    }
}
