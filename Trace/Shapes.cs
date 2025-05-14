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
    public static Normal OrientedNormal(Point p, Vec rayDir)
    {
        var result = new Normal(p.X, p.Y, p.Z);
        return p.to_vec() * rayDir < 0 ? result : -result;
    }
    
    public abstract Vec2d ShapePointToUV(Point p);
    public abstract bool QuickRayIntersection(Ray ray);
    public abstract HitRecord? RayIntersection(Ray ray);
    public abstract bool IsPointInternals(Point p, Ray ray);
}

public class Sphere : Shape
{
    public float Radius { get; }

    /// <summary>
    /// Creates a unit‐sphere (radius=1) at the origin, or with an optional
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
    public override Vec2d ShapePointToUV(Point p)
    {
        // atan2 returns an angle in [-π, +π]
        var u = MathF.Atan2(p.Y, p.X) / (2 * MathF.PI);
        if (u < 0)
            u += 1.0f;     // remap to [0,1)

        // acos returns an angle in [0, π], so v is in [0,1]
        var v = MathF.Acos(p.Z) / MathF.PI;

        return new Vec2d(u, v);
    }

    /// <summary>
    /// Returns true if ray intersects sphere. Returns false if it doesn't.
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public override bool QuickRayIntersection(Ray ray)
    {
        var localRay = ray.Transform(Transformation.Inverse());

        var origin    = localRay.Origin.to_vec();
        var direction = localRay.Direction;

        var a = direction.SqNorm();
        var b = 2 * origin * direction;
        var c = origin.SqNorm() - Radius * Radius;
        
        var delta = b * b - 4 * a * c;
        if (delta < 0)
            return false;
        
        var sqrtDelta = MathF.Sqrt(delta);
        var tMin       = (-b - sqrtDelta) / (2 * a);
        var tMax     = (-b + sqrtDelta) / (2 * a);

        return (localRay.Tmin < tMin && tMin < localRay.Tmax) || (localRay.Tmin < tMax && tMax < localRay.Tmax);

    }

    /// <summary>
    /// Checks if a ray intersects the sphere
    /// Returns a `HitRecord`, or `null` if no intersection was found.
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public override HitRecord? RayIntersection(Ray ray)
    {
        var localRay = ray.Transform(Transformation.Inverse());

        var origin    = localRay.Origin.to_vec();
        var direction = localRay.Direction;

        var a = direction.SqNorm();
        var b = 2 * origin * direction;
        var c = origin.SqNorm() - Radius * Radius;

        var delta = b * b - 4 * a * c;
        if (delta < 0)
            return null;

        var sqrtDelta = MathF.Sqrt(delta);
        var tMin       = (-b - sqrtDelta) / (2 * a);
        var tMax     = (-b + sqrtDelta) / (2 * a);

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
        Vec2d surfaceUV = ShapePointToUV(localHit);

        // Transform the intersection point and normal back to world space
        Point worldPoint   = Transformation * localHit;
        Normal worldNormal = Transformation * localNormal;

        return new HitRecord
        {
            WorldPoint   = worldPoint,
            Normal       = worldNormal,
            SurfacePoint = surfaceUV,
            t            = firstHit,
            Ray          = ray,
        };
    }

    public override bool IsPointInternals(Point p, Ray ray)
    {
        if(!QuickRayIntersection(ray))
        {
            //throw new Exception("ray does not intersect sphere");
            return false;
        }
        var localRay = ray.Transform(Transformation.Inverse());

        var origin    = localRay.Origin.to_vec();
        var direction = localRay.Direction;

        var a = direction.SqNorm();
        var b = 2 * origin * direction;
        var c = origin.SqNorm() - Radius * Radius;

        var delta = b * b - 4 * a * c;
        if (delta < 0)
            return false;

        var sqrtDelta = MathF.Sqrt(delta);
        var tMin       = (-b - sqrtDelta) / (2 * a);
        var tMax     = (-b + sqrtDelta) / (2 * a);

        return Math.Abs(ray.PointAt(tMin).X) < Math.Abs(p.X) && Math.Abs(ray.PointAt(tMax).X) > Math.Abs(p.X) &&
               Math.Abs(ray.PointAt(tMin).Y) < Math.Abs(p.Y) && Math.Abs(ray.PointAt(tMax).Y) > Math.Abs(p.Y) &&
               Math.Abs(ray.PointAt(tMin).Z) < Math.Abs(p.Z) && Math.Abs(ray.PointAt(tMax).Z) > Math.Abs(p.Z);
    }
}

public class Plane : Shape
{
    public Plane(Transformation? transformation = null)
        : base(transformation)
    {
    }

    public override Vec2d ShapePointToUV(Point p)
    {
        return new Vec2d(p.X - (float)Math.Floor(p.X), p.Y - (float)Math.Floor(p.Y));
    }

    public override bool QuickRayIntersection(Ray ray)
    {
        var localRay = ray.Transform(Transformation.Inverse());

        var t = -localRay.Origin.Z / localRay.Direction.Z;

        return !(t <= localRay.Tmin) && !(t >= localRay.Tmax) && !(Math.Abs(localRay.Direction.Z) < 1E-5);
    }

    /// <summary>
    /// Checks if a ray intersects the plane
    /// Returns a `HitRecord`, or `null` if no intersection was found.
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public override HitRecord? RayIntersection(Ray ray)
    {
        var localRay = ray.Transform(Transformation.Inverse());

        if (Math.Abs(localRay.Direction.Z) < 1E-5) return null;

        var t = -localRay.Origin.Z / localRay.Direction.Z;

        if (t <= localRay.Tmin || t >= localRay.Tmax) return null;

        var localHit = localRay.PointAt(t);

        var localNormal = OrientedNormal(new Point(0.0f, 0.0f, 1.0f), localRay.Direction);

        return new HitRecord
        {
            WorldPoint   = Transformation * localHit,
            Normal       = Transformation * localNormal,
            SurfacePoint = ShapePointToUV(localHit),
            t            = -localRay.Origin.Z / localRay.Direction.Z,
            Ray          = ray,
    };

    }
    
    public override bool IsPointInternals(Point p, Ray ray)
    {
        
        return true;
    }
}

public class Composition
{
    public List<Shape> Shapes { get; set; } = [];

    public void AddShape(Shape shape)
    {
        Shapes.Add(shape);
    }

    public HitRecord Union(Ray ray)
    {
        var closest = new HitRecord();
        foreach (var intersection in Shapes.Select(shape => shape.RayIntersection(ray)).OfType<HitRecord>().Where(intersection => closest == null || intersection.t < closest.t))
        {
            closest = intersection;
        }
        return closest;
    }
    
    /*
    public HitRecord Difference(Ray ray)
    {
        var intersections = (from t in Shapes where t.QuickRayIntersection(ray) select t.RayIntersection(ray)).ToList();

        var T = .0f;
        for (var i = 0; i < intersections.Count; i++)
        {
            if(intersections[i].t  T) continue;
        }

        return intersection;
    }
    */
}