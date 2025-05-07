/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

namespace Trace;

public abstract class Shape
{
    public Transformation Transformation { get; }

    public Shape(Transformation? transformation = null)
    {
        Transformation = transformation ?? new Transformation();
    }

    /// <summary>
    /// Ensures that the normal is oriented outward relative to the ray.
    /// If dot(normal, rayDir) > 0, the normal points in the same direction
    /// as the ray (backface), so it is inverted.
    /// </summary>
    public Normal OrientedNormal(Point p, Vec rayDir)
    {
        var result = new Normal(p.X, p.Y, p.Z);
        return p.to_vec() * rayDir < 0 ? result : -result;
    }

    public abstract HitRecord? RayIntersection(Ray ray);
}

public class Sphere : Shape
{
    public float Radius { get; }

    /// <summary>
    /// Creates an unit‐sphere (radius=1) at the origin, or with an optional
    /// radius and/or transformation.
    /// </summary>
    /// <param name="radius">Sphere radius (default 1.0f).</param>
    /// <param name="transformation">Optional transform (default identity).</param>
    public Sphere(float radius = 1.0f, Transformation? transformation = null)
        : base(transformation)
    {
        Radius = radius;
    }


    /// <summary>
    /// Maps a point on the unit sphere (local coordinates) to UV texture coordinates.
    /// u in [0,1) is calculated via atan2(y, x), v in [0,1] via acos(z).
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static Vec2d SpherePointToUV(Point p)
    {
        // atan2 returns an angle in [-π, +π]
        var u = MathF.Atan2(p.Y, p.X) / (2 * MathF.PI);
        if (u < 0)
            u += 1.0f; // remap to [0,1)

        // acos returns an angle in [0, π], so v is in [0,1]
        var v = MathF.Acos(p.Z) / MathF.PI;

        return new Vec2d(u, v);
    }

    public override HitRecord? RayIntersection(Ray ray)
    {
        var localRay = ray.Transform(Transformation.Inverse());

        var origin = localRay.Origin.to_vec();
        var direction = localRay.Direction;

        var a = direction.SqNorm();
        var b = 2 * origin * direction;
        var c = origin.SqNorm() - Radius * Radius;

        var delta = b * b - 4 * a * c;
        if (delta < 0)
            return null;

        var sqrtDelta = MathF.Sqrt(delta);
        var tMin = (-b - sqrtDelta) / (2 * a);
        float tMax = (-b + sqrtDelta) / (2 * a);

        float firstHit;
        if (tMin > localRay.Tmin && tMin < localRay.Tmax)
            firstHit = tMin;
        else if (tMax > localRay.Tmin && tMax < localRay.Tmax)
            firstHit = tMax;
        else
            return null;

        var localHit = localRay.PointAt(firstHit);

        // Calculate the local normal (flipped if necessary)
        Normal localNormal = OrientedNormal(localHit, localRay.Direction);

        // Compute UV texture coordinates
        Vec2d surfaceUV = SpherePointToUV(localHit);

        // Transform the intersection point and normal back to world space
        Point worldPoint = Transformation * localHit;
        Normal worldNormal = Transformation * localNormal;

        return new HitRecord
        {
            WorldPoint = worldPoint,
            Normal = worldNormal,
            SurfacePoint = surfaceUV,
            t = firstHit,
            Ray = ray,
        };
    }

}

public class Plane : Shape
{
    /// <summary>
    /// Creates an infinite plane in the XY-plane (Z = 0), optionally transformed.
    /// </summary>
    public Plane(Transformation? transformation = null)
        : base(transformation)
    {
    }

    public override HitRecord? RayIntersection(Ray ray)
    {
        // 1) Transform the ray into the plane’s local space
        Ray localRay = ray.Transform(Transformation.Inverse());

        // 2) If the ray is parallel to the plane, no hit
        if (MathF.Abs(localRay.Direction.Z) < 1e-5f)
            return null;

        // 3) Compute intersection t with the Z=0 plane: origin.Z + t*dir.Z = 0
        var t = -localRay.Origin.Z / localRay.Direction.Z;

        // 4) Reject if outside the valid t-range
        if (t <= localRay.Tmin || t >= localRay.Tmax)
            return null;

        // 5) Compute the local-space hit point
        var localHit = localRay.PointAt(t);

        // 6) Compute the local normal (±Z) so it always faces the ray
        Normal localNormal = localRay.Direction.Z < 0
            ? new Normal(0f, 0f, 1f)
            : new Normal(0f, 0f, -1f);

        // 7) Map local hit point to UV by frac(x), frac(y)
        float u = localHit.X - MathF.Floor(localHit.X);
        float v = localHit.Y - MathF.Floor(localHit.Y);
        Vec2d uv = new Vec2d(u, v);

        // 8) Transform point and normal back to world space
        Point worldPoint  = Transformation * localHit;
        Normal worldNormal = Transformation * localNormal;

        // 9) Return the fully populated HitRecord
        return new HitRecord(
            worldPoint,
            worldNormal,
            uv,
            t,
            ray
        );
    }
}