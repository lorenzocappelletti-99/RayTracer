/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/
using System;
using System.Collections.Generic;
using System.Linq; 


namespace Trace;


public abstract class Shape
{
    /// <summary>
    /// World‐to‐object transformation.
    /// </summary>
    public Transformation Transformation { get; }

    /// <summary>
    /// Surface material (optional).
    /// </summary>
    public Material? Material { get; }

    /// <summary>
    /// Initializes a new shape with an optional material and optional transform.
    /// </summary>
    /// <param name="material">Surface material (optional).</param>
    /// <param name="transformation">World‐to‐object transform (optional).</param>
    public Shape(Material? material = null, Transformation? transformation = null)
    {
        Material       = material;
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
    public abstract bool IsPointInternal(Point p);
}

public class Sphere : Shape
{
    /// <summary>
    /// Sphere radius.
    /// </summary>
    public float Radius { get; }

    /// <summary>
    /// Creates a sphere with optional radius, material and transform.
    /// </summary>
    /// <param name="radius">Sphere radius (default=1.0f).</param>
    /// <param name="material">Surface material (optional).</param>
    /// <param name="transformation">World‐to‐object transform (optional).</param>
    public Sphere(
        float radius = 1.0f,
        Transformation? transformation = null,
        Material? material = null)
        : base(material, transformation)
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
            T            = firstHit,
            Ray          = ray,
            Material      = Material
        };
    }

    public override bool IsPointInternal(Point p)
    {
        var ray = new Ray(p, Vec.VEC_X);
        var localRay = ray.Transform(Transformation.Inverse());

        var origin    = localRay.Origin.to_vec();
        var direction = localRay.Direction;

        var a = direction.SqNorm();
        var b = 2 * origin * direction;
        var c = origin.SqNorm() - Radius * Radius;
        
        var delta = b * b - 4 * a * c;
        if (p.AreClose(delta, 0)) return true;
        
        var sqrtDelta = MathF.Sqrt(delta);
        var tMin       = (-b - sqrtDelta) / (2 * a);
        var tMax     = (-b + sqrtDelta) / (2 * a);
        
        return ray.PointAt(tMin).X <= p.X && ray.PointAt(tMax).X >= p.X &&
               ray.PointAt(tMin).Y <= p.Y && ray.PointAt(tMax).Y >= p.Y &&
               ray.PointAt(tMin).Z <= p.Z && ray.PointAt(tMax).Z >= p.Z;
    }
}

public class Plane : Shape
{
    public Plane(
        Material? material = null,
        Transformation? transformation = null)
        : base(material, transformation)
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
            T            = -localRay.Origin.Z / localRay.Direction.Z,
            Ray          = ray,
            Material      = Material
    };

    }
    
    public override bool IsPointInternal(Point p)
    {
        return false;
    }
}

public enum CsgOperation
{
    Union,
    Intersection,
    Difference
}

public class Csg : Shape
    {
        public readonly Shape First;
        public readonly Shape Second;
        public readonly CsgOperation Op;

        public Csg(Shape first, Shape second, CsgOperation operation)
            : base(material: null, transformation: null)
        {
            First  = first;
            Second = second;
            Op     = operation;
        }

        public override Vec2d ShapePointToUV(Point p)
        {
            // UV is taken from whichever shape provided the hit;
            // handled in RayIntersection
            return new Vec2d();
        }

        public override bool QuickRayIntersection(Ray ray)
        {
            // If either child might intersect, we consider there might be a hit.
            return First.QuickRayIntersection(ray) || Second.QuickRayIntersection(ray);
        }

        public override HitRecord? RayIntersection(Ray ray)
        {
            if (!QuickRayIntersection(ray))
                return null;
            
            // Gather all hits from both children
            var firstHits  = First.RayIntersection(ray);
            var secondHits = Second.RayIntersection(ray);

            // Merge and sort by t
            var events = new List<(float T, HitRecord Hit, bool IsFirst)>();

            var h1 = First.RayIntersection(ray);
            if (h1 != null) events.Add((h1.T, h1, true));

            var h2 = Second.RayIntersection(ray);
            if (h2 != null) events.Add((h2.T, h2, false));

            events.Sort((a, b) => a.T.CompareTo(b.T));
            

            // Determine initial inside-state just before first event
            float epsilon = 1e-4f;
            float t0 = (events.Count > 0 ? events[0].T : float.MaxValue) - epsilon;
            var p0 = ray.PointAt(t0);
            bool inFirst  = First.IsPointInternal(p0);
            bool inSecond = Second.IsPointInternal(p0);

            // Sweep through events to find first intersection according to operation
            foreach (var (t, hit, isFirst) in events)
            {
                // Toggle state for the shape that generated this event
                if (isFirst) inFirst = !inFirst;
                else         inSecond = !inSecond;

                bool inside = Op switch
                {
                    CsgOperation.Union        => inFirst || inSecond,
                    CsgOperation.Intersection => inFirst && inSecond,
                    CsgOperation.Difference   => inFirst && !inSecond,
                    _                          => false
                };

                if (inside)
                {
                    // Return the corresponding hit record
                    return hit;
                }
            }

            return null;
        }

        public override bool IsPointInternal(Point p)
        {
            var insideFirst  = First.IsPointInternal(p);
            var insideSecond = Second.IsPointInternal(p);

            return Op switch
            {
                CsgOperation.Union        => insideFirst || insideSecond,
                CsgOperation.Intersection => insideFirst && insideSecond,
                CsgOperation.Difference   => insideFirst && !insideSecond,
                _                          => false
            };
        }

        private static HitRecord? PickNearest(HitRecord? a, HitRecord? b)
        {
            if (a == null) return b;
            if (b == null) return a;
            return a.T < b.T ? a : b;
        }
    }



/*
public class Composition
{
    public List<Shape> ShapesAdd { get; } = [];
    public List<Shape> ShapesDif { get; } = [];

    public void AddShapeAdd(Shape shape)
    {
        ShapesAdd.Add(shape);
    }
    public void AddShapeDif(Shape shape)
    {
        ShapesDif.Add(shape);
    }

    public HitRecord Union(Ray ray)
    {
        var closest = new HitRecord();
        foreach (var intersection in ShapesAdd.Select(shape => shape.RayIntersection(ray))
                     .OfType<HitRecord>()
                     .Where(intersection => closest == null || intersection.T < closest.T))
        {
            closest = intersection;
        }
        return closest;
    }
    
    
    public HitRecord Difference(Ray ray)
    {
        var intersectionsAdd = (from t in ShapesAdd where t.QuickRayIntersection(ray) select t.RayIntersection(ray)).ToList();
        var intersectionsDif = (from t in ShapesDif where t.QuickRayIntersection(ray) select t.RayIntersection(ray)).ToList();

        var myHits = new List<HitRecord>();
        
        foreach (var interDif in intersectionsDif)
        {
            foreach (var shapeAdd in ShapesAdd)
            {
                if (shapeAdd.IsPointInternal(interDif.WorldPoint)) myHits.Add(interDif);
            }
        }

        foreach (var interAdd in intersectionsAdd)
        {
            foreach (var shapeAdd in ShapesDif)
            {
                if (!shapeAdd.IsPointInternal(interAdd.WorldPoint)) myHits.Add(interAdd);
            }
        }
        
        return myHits.OrderBy(h => h.T).First();
    }
}
*/