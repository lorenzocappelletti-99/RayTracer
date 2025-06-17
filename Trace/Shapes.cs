/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using System.Diagnostics;


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

/***********************************************************
 *                                                         *
 *                        SPHERE                           *
 *                                                         *
 ***********************************************************/

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

/***********************************************************
 *                                                         *
 *                        PLANE                            *
 *                                                         *
 ***********************************************************/

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




/***********************************************************
 *                                                         *
 *                     RECTANGLE                           *
 *                                                         *
 ***********************************************************/
public class Rectangle : Shape
{
    public float Width { get; }
    public float Height { get; }

    /// <summary>
    /// Initializes a new instance of the Rectangle class, representing a finite rectangular surface.
    /// The rectangle is in its local XY-plane (Z=0), centered at the origin.
    /// </summary>
    /// <param name="width">The width of the rectangle along the local X-axis.</param>
    /// <param name="height">The height of the rectangle along the local Y-axis.</param>
    /// <param name="material">The material of the rectangle.</param>
    /// <param name="transformation">The transformation applied to the rectangle.</param>
    public Rectangle(
        float width,
        float height,
        Material? material = null,
        Transformation? transformation = null)
        : base(material, transformation)
    {
        // Basic validation for dimensions
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentException("Width and Height must be positive values for a finite rectangle.");
        }
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Maps a local 3D point on the rectangle to 2D UV coordinates (0-1 range).
    /// </summary>
    /// <param name="p">The local 3D point on the rectangle (Z=0).</param>
    /// <returns>A Vec2d containing the U and V coordinates.</returns>
    public override Vec2d ShapePointToUV(Point p)
    {
        // Map local X from [-Width/2, Width/2] to U [0, 1]
        var u = (p.X + (Width / 2.0f)) / Width;
        // Map local Y from [-Height/2, Height/2] to V [0, 1]
        var v = (p.Y + (Height / 2.0f)) / Height;
        return new Vec2d(u, v);
    }

    /// <summary>
    /// Performs a quick check for ray intersection with the finite rectangle.
    /// </summary>
    /// <param name="ray">The ray in world coordinates.</param>
    /// <returns>True if the ray intersects the finite rectangle within its bounds and Tmin/Tmax, false otherwise.</returns>
    public override bool QuickRayIntersection(Ray ray)
    {
        var localRay = ray.Transform(Transformation.Inverse());

        // Check if ray is parallel to the rectangle's plane (or nearly so)
        if (Math.Abs(localRay.Direction.Z) < 1E-5) return false;

        // Calculate 't' for intersection with the infinite plane (Z=0)
        var t = -localRay.Origin.Z / localRay.Direction.Z;

        // Check if 't' is within the ray's valid range
        if (t <= localRay.Tmin || t >= localRay.Tmax) return false;

        // Calculate the local hit point on the infinite plane
        var localHit = localRay.PointAt(t);

        // --- Check if the local hit point falls within the rectangle's bounds ---
        var halfWidth = Width / 2.0f;
        var halfHeight = Height / 2.0f;

        if (localHit.X < -halfWidth || localHit.X > halfWidth ||
            localHit.Y < -halfHeight || localHit.Y > halfHeight)
        {
            return false; // Intersection point is outside the rectangle's bounds
        }

        return true; // Intersection found within bounds and ray limits
    }

    /// <summary>
    /// Checks if a ray intersects the finite rectangle and returns a `HitRecord`.
    /// Returns `null` if no intersection was found.
    /// </summary>
    /// <param name="ray">The ray in world coordinates.</param>
    /// <returns>A HitRecord if an intersection was found, or null.</returns>
    public override HitRecord? RayIntersection(Ray ray)
    {
        var localRay = ray.Transform(Transformation.Inverse());

        // Check if ray is parallel to the rectangle's plane (or nearly so)
        if (Math.Abs(localRay.Direction.Z) < 1E-5) return null;

        // Calculate 't' for intersection with the infinite plane (Z=0)
        var t = -localRay.Origin.Z / localRay.Direction.Z;

        // Check if 't' is within the ray's valid range
        if (t <= ray.Tmin || t >= ray.Tmax) return null; // Use original ray's Tmin/Tmax

        // Calculate the local hit point on the infinite plane
        var localHit = localRay.PointAt(t);

        // --- Check if the local hit point falls within the rectangle's bounds ---
        var halfWidth = Width / 2.0f;
        var halfHeight = Height / 2.0f;

        if (localHit.X < -halfWidth || localHit.X > halfWidth ||
            localHit.Y < -halfHeight || localHit.Y > halfHeight)
        {
            return null; // Intersection point is outside the rectangle's bounds
        }

        // The normal of a plane at Z=0 in local space is (0,0,1).
        // Use OrientedNormal to ensure the normal points towards the ray origin if needed.
        var localNormal = OrientedNormal(new Point(0.0f, 0.0f, 1.0f), localRay.Direction);

        return new HitRecord
        {
            WorldPoint   = Transformation * localHit, // Transform local hit point back to world space
            Normal       = Transformation * localNormal,   // Transform local normal to world space
            SurfacePoint = ShapePointToUV(localHit), // Get UV coordinates for material mapping
            T            = t,                                   // The 't' value for the intersection
            Ray          = ray,                               // The original ray
            Material     = Material                      // The material of this rectangle
        };
    }

    /// <summary>
    /// Determines if a world point is internal to the rectangle.
    /// For a finite, non-solid surface like a rectangle, this typically returns false.
    /// </summary>
    /// <param name="p">The point to check.</param>
    /// <returns>False, as a rectangle is not a solid volume.</returns>
    public override bool IsPointInternal(Point p)
    {
        return false;
    }
}


/***********************************************************
 *                                                         *
 *                     HALF SPACE                          *
 *                                                         *
 ***********************************************************/

public class Box : Shape
{
    /// <summary>
    /// Minimum corner of the box in local coordinates.
    /// </summary>
    public Vec Min { get; }

    /// <summary>
    /// Maximum corner of the box in local coordinates.
    /// </summary>
    public Vec Max { get; }

    /// <summary>
    /// Creates an axis-aligned box (parallelepiped) with specified min/max bounds, material and transform.
    /// </summary>
    /// <param name="min">Local-space minimum corner (default = (-1,-1,-1)).</param>
    /// <param name="max">Local-space maximum corner (default = (1,1,1)).</param>
    /// <param name="transformation">Optional world-to-object transform.</param>
    /// <param name="material">Optional surface material.</param>
    public Box(
        Vec? min             = null,
        Vec? max             = null,
        Transformation? transformation = null,
        Material? material   = null)
        : base(material, transformation)
    {
        // Se non passo nulla, uso i valori di default
        Min = min ?? new Vec(-1f, -1f, -1f);
        Max = max ?? new Vec( 1f,  1f,  1f);
    }

    /// <summary>
    /// Maps a point on the box surface (local coordinates) to UV texture coordinates.
    /// UV layout depends on which face is hit.
    /// </summary>
    public override Vec2d ShapePointToUV(Point p)
    {
        // For simplicity, check face by comparing distances to planes
        double u = 0, v = 0;
        if (Math.Abs(p.X - Min.X) < 1e-6)
        {
            // Left face: y, z
            u = (p.Z - Min.Z) / (Max.Z - Min.Z);
            v = (p.Y - Min.Y) / (Max.Y - Min.Y);
        }
        else if (Math.Abs(p.X - Max.X) < 1e-6)
        {
            // Right face
            u = 1 - (p.Z - Min.Z) / (Max.Z - Min.Z);
            v = (p.Y - Min.Y) / (Max.Y - Min.Y);
        }
        else if (Math.Abs(p.Y - Min.Y) < 1e-6)
        {
            // Bottom face: x, z
            u = (p.X - Min.X) / (Max.X - Min.X);
            v = (p.Z - Min.Z) / (Max.Z - Min.Z);
        }
        else if (Math.Abs(p.Y - Max.Y) < 1e-6)
        {
            // Top face
            u = (p.X - Min.X) / (Max.X - Min.X);
            v = 1 - (p.Z - Min.Z) / (Max.Z - Min.Z);
        }
        else if (Math.Abs(p.Z - Min.Z) < 1e-6)
        {
            // Front face: x, y
            u = (p.X - Min.X) / (Max.X - Min.X);
            v = (p.Y - Min.Y) / (Max.Y - Min.Y);
        }
        else if (Math.Abs(p.Z - Max.Z) < 1e-6)
        {
            // Back face
            u = 1 - (p.X - Min.X) / (Max.X - Min.X);
            v = (p.Y - Min.Y) / (Max.Y - Min.Y);
        }
        return new Vec2d((float)u, (float)v);
    }

    /// <summary>
    /// Quick test for ray intersection using slab method.
    /// </summary>
    public override bool QuickRayIntersection(Ray ray)
    {
        var localRay = ray.Transform(Transformation.Inverse());
        var origin = localRay.Origin.to_vec();
        var dir = localRay.Direction;

        // Slab method
        float tMin = (Min.X - origin.X) / dir.X;
        float tMax = (Max.X - origin.X) / dir.X;
        if (tMin > tMax) (tMin, tMax) = (tMax, tMin);

        float tyMin = (Min.Y - origin.Y) / dir.Y;
        float tyMax = (Max.Y - origin.Y) / dir.Y;
        if (tyMin > tyMax) (tyMin, tyMax) = (tyMax, tyMin);

        if (tMin > tyMax || tyMin > tMax)
            return false;
        tMin = MathF.Max(tMin, tyMin);
        tMax = MathF.Min(tMax, tyMax);

        float tzMin = (Min.Z - origin.Z) / dir.Z;
        float tzMax = (Max.Z - origin.Z) / dir.Z;
        if (tzMin > tzMax) (tzMin, tzMax) = (tzMax, tzMin);

        if (tMin > tzMax || tzMin > tMax)
            return false;

        tMin = MathF.Max(tMin, tzMin);
        tMax = MathF.Min(tMax, tzMax);

        return (localRay.Tmin < tMin && tMin < localRay.Tmax) ||
               (localRay.Tmin < tMax && tMax < localRay.Tmax);
    }

    /// <summary>
    /// Detailed ray intersection, returns HitRecord or null.
    /// </summary>
    public override HitRecord? RayIntersection(Ray ray)
    {
        var localRay = ray.Transform(Transformation.Inverse());
        var origin = localRay.Origin.to_vec();
        var dir = localRay.Direction;

        float tMin = (Min.X - origin.X) / dir.X;
        float tMax = (Max.X - origin.X) / dir.X;
        if (tMin > tMax)
            (tMin, tMax) = (tMax, tMin);

        float tyMin = (Min.Y - origin.Y) / dir.Y;
        float tyMax = (Max.Y - origin.Y) / dir.Y;
        if (tyMin > tyMax) (tyMin, tyMax) = (tyMax, tyMin);

        if (tMin > tyMax || tyMin > tMax)
            return null;
        tMin = MathF.Max(tMin, tyMin);
        tMax = MathF.Min(tMax, tyMax);

        float tzMin = (Min.Z - origin.Z) / dir.Z;
        float tzMax = (Max.Z - origin.Z) / dir.Z;
        if (tzMin > tzMax) (tzMin, tzMax) = (tzMax, tzMin);

        if (tMin > tzMax || tzMin > tMax)
            return null;
        tMin = MathF.Max(tMin, tzMin);
        tMax = MathF.Min(tMax, tzMax);

        float tHit = tMin > localRay.Tmin && tMin < localRay.Tmax ? tMin :
                     (tMax > localRay.Tmin && tMax < localRay.Tmax ? tMax : float.NaN);
        if (float.IsNaN(tHit))
            return null;

        var localHit = localRay.PointAt(tHit);
        // Compute local normal
        Vec n = new Vec(
            Math.Abs(localHit.X - Min.X) < 1e-5 ? -1 : (Math.Abs(localHit.X - Max.X) < 1e-6 ? 1 : 0),
            Math.Abs(localHit.Y - Min.Y) < 1e-5 ? -1 : (Math.Abs(localHit.Y - Max.Y) < 1e-6 ? 1 : 0),
            Math.Abs(localHit.Z - Min.Z) < 1e-5 ? -1 : (Math.Abs(localHit.Z - Max.Z) < 1e-6 ? 1 : 0)
        );
        var localNormal = new Normal(n.X,n.Y,n.Z);

        // UV coords
        var uv = ShapePointToUV(localHit);
        
        // Transform back
        Point worldPoint = Transformation * localHit;
        Normal worldNormal = Transformation * localNormal;

        return new HitRecord
        {
            WorldPoint = worldPoint,
            Normal = worldNormal,
            SurfacePoint = uv,
            T = tHit,
            Ray = ray,
            Material = Material
        };
    }

    /// <summary>
    /// Checks if a point is inside the box (inclusive).
    /// </summary>
    public override bool IsPointInternal(Point p)
    {
        var local = (Transformation.Inverse() * p).to_vec();
        return local.X >= Min.X && local.X <= Max.X &&
               local.Y >= Min.Y && local.Y <= Max.Y &&
               local.Z >= Min.Z && local.Z <= Max.Z;
    }
}



/***********************************************************
 *                                                         *
 *                        TORUS                            *
 *                                                         *
 ***********************************************************/

public class Torus : Shape
{
    /// <summary>
    /// Major radius (distance from center to the middle of the tube).
    /// Minor radius (radius of the tube itself).
    /// </summary>
    public float MajorRadius { get; }
    public float MinorRadius { get; }

    public Torus(
        float majorRadius = 1.0f,
        float minorRadius = 0.5f,
        Transformation? transformation = null,
        Material? material = null)
        : base(material, transformation)
    {
        MajorRadius = majorRadius;
        MinorRadius = minorRadius;
    }

    public override Vec2d ShapePointToUV(Point p)
    {
        // Convert point to cylindrical coordinates around the Y axis
        var theta = MathF.Atan2(p.Z, p.X); // angle around Y axis
        var r = MathF.Sqrt(p.X * p.X + p.Z * p.Z);
        var phi = MathF.Atan2(p.Y, r - MajorRadius); // angle around the tube

        float u = (theta + MathF.PI) / (2 * MathF.PI);
        float v = (phi + MathF.PI) / (2 * MathF.PI);

        return new Vec2d(u, v);
    }

    public override bool QuickRayIntersection(Ray ray)
    {
        return RayIntersection(ray) != null;
    }

    public override HitRecord? RayIntersection(Ray ray)
    {
        var localRay = ray.Transform(Transformation.Inverse());
        var o = localRay.Origin.to_vec();
        var d = localRay.Direction;

        // Solving torus equation by substituting ray into implicit form of torus:
        // Torus: (|p|^2 + R^2 - r^2)^2 - 4R^2(x^2 + z^2) = 0
        // Ray: p(t) = o + td → substitute and expand into quartic
        var ox = o.X; var oy = o.Y; var oz = o.Z;
        var dx = d.X; var dy = d.Y; var dz = d.Z;
        var R = MajorRadius;
        var r = MinorRadius;

        var sumDSq = dx * dx + dy * dy + dz * dz;
        var e = ox * ox + oy * oy + oz * oz - R * R - r * r;
        var f = ox * dx + oy * dy + oz * dz;

        var fourR2 = 4 * R * R;

        // Coefficients of the quartic equation At^4 + Bt^3 + Ct^2 + Dt + E = 0
        var A = sumDSq * sumDSq;
        var B = 4 * sumDSq * f;
        var C = 2 * sumDSq * e + 4 * f * f + fourR2 * dy * dy;
        var D = 4 * f * e + 2 * fourR2 * oy * dy;
        var E = e * e - fourR2 * (r * r - oy * oy);

        var roots = SolveQuartic(A, B, C, D, E); 

        float tHit = float.PositiveInfinity;
        foreach (var t in roots)
        {
            if (t > localRay.Tmin && t < localRay.Tmax && t < tHit)
                tHit = t;
        }

        if (float.IsInfinity(tHit)) return null;

        var localHit = localRay.PointAt(tHit);
        var localNormal = OrientedNormal(localHit, localRay.Direction);
        var surfaceUV = ShapePointToUV(localHit);

        var worldPoint = Transformation * localHit;
        var worldNormal = Transformation * localNormal;

        return new HitRecord
        {
            WorldPoint = worldPoint,
            Normal = worldNormal,
            SurfacePoint = surfaceUV,
            T = tHit,
            Ray = ray,
            Material = Material
        };
    }

    public override bool IsPointInternal(Point p)
    {
        var localP = Transformation.Inverse() * p;
        var xzLen = MathF.Sqrt(localP.X * localP.X + localP.Z * localP.Z);
        var distFromTubeCenter = MathF.Sqrt((xzLen - MajorRadius) * (xzLen - MajorRadius) + localP.Y * localP.Y);
        return distFromTubeCenter < MinorRadius;
    }
    
    public static List<float> SolveQuartic(float A, float B, float C, float D, float E)
    {
        const float EPSILON = 1e-6f;

        // Normalize if A != 1
        if (MathF.Abs(A) < EPSILON)
        {
            // Reduce to cubic
            return SolveCubic(B, C, D, E);
        }

        float a = B / A;
        float b = C / A;
        float c = D / A;
        float d = E / A;

        // Depressed quartic: x⁴ + px² + qx + r = 0
        float a2 = a * a;
        float p = -0.375f * a2 + b;
        float q = 0.125f * a2 * a - 0.5f * a * b + c;
        float r = -0.01171875f * a2 * a2 + 0.0625f * a2 * b - 0.25f * a * c + d;

        List<float> roots = new();

        if (MathF.Abs(q) < EPSILON)
        {
            // Biquadratic: y² + p y + r = 0, let y = x²
            var yRoots = SolveQuadratic(1, p, r);
            foreach (var y in yRoots)
            {
                if (y < -EPSILON) continue;
                var sqrtY = MathF.Sqrt(y);
                roots.Add(sqrtY - 0.25f * a);
                roots.Add(-sqrtY - 0.25f * a);
            }
        }
        else
        {
            // Solve resolvent cubic: z³ - 0.5p z² - r z + (0.125 r p - 0.5 q²) = 0
            float cubicA = 1;
            float cubicB = -0.5f * p;
            float cubicC = -r;
            float cubicD = 0.125f * r * p - 0.5f * q * q;

            var zRoots = SolveCubic(cubicA, cubicB, cubicC, cubicD);
            float z = zRoots[0]; // one real root

            float u = MathF.Sqrt(2 * z - p);
            if (MathF.Abs(u) < EPSILON)
                return new(); // avoid division by zero

            float v = q / (2 * u);

            var quad1 = SolveQuadratic(1, u, z - v);
            var quad2 = SolveQuadratic(1, -u, z + v);

            foreach (var root in quad1.Concat(quad2))
            {
                roots.Add(root - 0.25f * a);
            }
        }

        roots.Sort();
        return roots.Where(r => !float.IsNaN(r) && !float.IsInfinity(r)).ToList();
    }
    
    public static List<float> SolveQuadratic(float a, float b, float c)
    {
        const float EPSILON = 1e-6f;
        List<float> roots = new();

        if (MathF.Abs(a) < EPSILON)
        {
            if (MathF.Abs(b) < EPSILON) return roots;
            roots.Add(-c / b);
            return roots;
        }

        float disc = b * b - 4 * a * c;
        if (disc < -EPSILON) return roots;

        if (disc < EPSILON)
        {
            roots.Add(-b / (2 * a));
        }
        else
        {
            float sqrtD = MathF.Sqrt(disc);
            float q = -0.5f * (b + MathF.CopySign(sqrtD, b));
            roots.Add(q / a);
            roots.Add(c / q);
        }

        return roots.Where(r => !float.IsNaN(r) && !float.IsInfinity(r)).ToList();
    }

    public static List<float> SolveCubic(float a, float b, float c, float d)
    {
        const float EPSILON = 1e-6f;
        List<float> roots = new();

        if (MathF.Abs(a) < EPSILON)
            return SolveQuadratic(b, c, d);

        float A = b / a;
        float B = c / a;
        float C = d / a;

        float sqA = A * A;
        float p = (1f / 3f) * (-1f / 3f * sqA + B);
        float q = (1f / 2f) * ((2f / 27f) * A * sqA - (1f / 3f) * A * B + C);

        float disc = q * q + p * p * p;

        if (MathF.Abs(disc) < EPSILON) disc = 0;

        float offset = -1f / 3f * A;

        if (disc > 0)
        {
            float sqrtDisc = MathF.Sqrt(disc);
            float u = MathF.Cbrt(-q + sqrtDisc);
            float v = MathF.Cbrt(-q - sqrtDisc);
            roots.Add(u + v + offset);
        }
        else if (disc == 0)
        {
            float u = MathF.Cbrt(-q);
            roots.Add(2 * u + offset);
            roots.Add(-u + offset);
        }
        else
        {
            float phi = MathF.Acos(-q / MathF.Sqrt(-p * p * p));
            float t = 2 * MathF.Sqrt(-p);

            roots.Add(t * MathF.Cos(phi / 3) + offset);
            roots.Add(t * MathF.Cos((phi + 2 * MathF.PI) / 3) + offset);
            roots.Add(t * MathF.Cos((phi + 4 * MathF.PI) / 3) + offset);
        }

        return roots.Where(r => !float.IsNaN(r) && !float.IsInfinity(r)).ToList();
    }


}





/***********************************************************
 *                                                         *
 *                        CONE                             *
 *                                                         *
 ***********************************************************/
public class Cone : Shape
{
    
    public float Radius { get; }
    public float Height { get; }

    
    public Cone(
        float radius,
        float height,
        Transformation? transformation = null,
        Material? material = null)
        : base(material, transformation)
    {
        Radius = radius;
        Height = height;
    }

    /// <summary>
    /// Maps a point on the cone to UV texture coordinates.
    /// This implementation is a simple cylindrical unwrap.
    /// </summary>
    public override Vec2d ShapePointToUV(Point p)
    {
        // Convert to cylindrical coords
        var theta = MathF.Atan2(p.Y, p.X);
        var u = theta / (2 * MathF.PI);
        if (u < 0) u += 1.0f; // remapped to 0,1

        var v = p.Z / Height;
        return new Vec2d(u, v);
    }

    public override bool QuickRayIntersection(Ray ray)
    {
        return RayIntersection(ray) != null;
    }

    public override HitRecord? RayIntersection(Ray ray)
    {
        var localRay = ray.Transform(Transformation.Inverse());

        var o = localRay.Origin.to_vec();
        var d = localRay.Direction;

        // Tip at origin, base at (0,0,h)
        var k = Radius / Height;
        var k2 = k * k;
        
        var a = d.X * d.X + d.Y * d.Y - k2 * d.Z * d.Z;
        var b = 2 * (d.X * o.X + d.Y * o.Y - k2 * d.Z * (o.Z - Height));
        var c = o.X * o.X + o.Y * o.Y - k2 * (o.Z - Height) * (o.Z - Height);



        var delta = b * b - 4 * a * c;
        if (delta < 0) return null;

        var sqrtDelta = MathF.Sqrt(delta);
        var t1 = (-b - sqrtDelta) / (2 * a);
        var t2 = (-b + sqrtDelta) / (2 * a);

        var t = float.MaxValue;
        foreach (var ti in new[] { t1, t2 })
        {
            if (ti > localRay.Tmin && ti < localRay.Tmax)
            {
                var hitZ = localRay.PointAt(ti).Z;
                if ((Height >= 0 && hitZ >= 0 && hitZ <= Height) ||
                    (Height < 0 && hitZ <= 0 && hitZ >= Height))
                {
                    t = ti;
                    break;
                }
            }
        }

        if (Math.Abs(t - float.MaxValue) < 1e-5)
        {
            // Try intersection with the base disk
            var denom = Vec.VEC_Z * d;
            if (MathF.Abs(denom) > 1e-6)
            {
                var tDisk = (Height - o.Z) / d.Z;
                if (tDisk > localRay.Tmin && tDisk < localRay.Tmax)
                {
                    var pDisk = localRay.PointAt(tDisk);
                    if (pDisk.X * pDisk.X + pDisk.Y * pDisk.Y <= Radius * Radius)
                    {
                        var worldPoint = Transformation * pDisk;
                        var worldNormal = Transformation * new Normal(0, 0, Height >= 0 ? 1 : -1);
                        var uv = new Vec2d(pDisk.X / Radius / 2 + 0.5f, pDisk.Y / Radius / 2 + 0.5f);

                        return new HitRecord
                        {
                            WorldPoint = worldPoint,
                            Normal = worldNormal,
                            SurfacePoint = uv,
                            T = tDisk,
                            Ray = ray,
                            Material = Material
                        };
                    }
                }
            }

            return null;
        }

        var localHit = localRay.PointAt(t);
        var normalDir = new Vec(localHit.X, localHit.Y, -k * MathF.Sqrt(localHit.X * localHit.X + localHit.Y * localHit.Y));
        normalDir = Height < 0 ? -normalDir : normalDir;
        normalDir.Normalize();
        var localNormal = normalDir.ToNorm();

        var uvCoords = ShapePointToUV(localHit);
        var worldPointFinal = Transformation * localHit;
        var worldNormalFinal = Transformation * localNormal;

        return new HitRecord
        {
            WorldPoint = worldPointFinal,
            Normal = worldNormalFinal,
            SurfacePoint = uvCoords,
            T = t,
            Ray = ray,
            Material = Material
        };
    }

    public override bool IsPointInternal(Point p)
    {
        var localPoint = (Transformation.Inverse()) * p;
        var rAtZ = (Radius / Height) * localPoint.Z;
        return localPoint.Z >= 0 && localPoint.Z <= Height &&
               localPoint.X * localPoint.X + localPoint.Y * localPoint.Y <= rAtZ * rAtZ;
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

        // 1) Calcola solo una volta le due intersezioni
        var h1 = First.RayIntersection(ray);
        var h2 = Second.RayIntersection(ray);

        // Se nessuna, esci subito
        if (h1 == null && h2 == null)
            return null;

        // 2) Determina l’ordine dei due eventi
        //   - Primo evento (tMin) e secondo evento (tMax)
        var (hitMin, isFirstMin) = 
            (h1 != null && (h2 == null || h1.T < h2.T)) ? (h1, true)
                : (h2, false);

        var (hitMax, isFirstMax) =
            (hitMin == h1) ? (h2, false) : (h1, true);

        // 3) Calcola stato “inside” all’inizio (just before tMin)
        Debug.Assert(hitMin != null, nameof(hitMin) + " != null");
        float t0 = hitMin.T - 1e-4f;
        var p0 = ray.PointAt(t0);
        bool inF = First.IsPointInternal(p0);
        bool inS = Second.IsPointInternal(p0);

        // 4) Processa il primo evento
        if (isFirstMin) inF = !inF; else inS = !inS;
        if (Op switch {
                CsgOperation.Union        => inF || inS,
                CsgOperation.Intersection => inF && inS,
                CsgOperation.Difference   => inF && !inS,
                _                          => false
            })
            return hitMin;

        // 5) Processa il secondo evento
        if (isFirstMax) inF = !inF; else inS = !inS;
        if (Op switch {
                CsgOperation.Union        => inF || inS,
                CsgOperation.Intersection => inF && inS,
                CsgOperation.Difference   => inF && !inS,
                _                          => false
            })
            return hitMax;

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
    
    public static Shape IntersectAll(params Shape[] shapes)
    {
        if (shapes.Length == 0) throw new ArgumentException(nameof(shapes));
        Shape result = shapes[0];
        for (int i = 1; i < shapes.Length; i++)
            result = new Csg(result, shapes[i], CsgOperation.Intersection);
        return result;
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