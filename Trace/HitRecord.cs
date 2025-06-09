/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using System;

namespace Trace
{
    /// <summary>
    /// Represents the result of a ray intersection with a shape, containing:
    /// - <see cref="WorldPoint"/>: the hit position in world coordinates
    /// - <see cref="Normal"/>: the surface normal at the hit point in world space
    /// - <see cref="SurfacePoint"/>: the UV texture coordinates at the hit point
    /// - <see cref="T"/>: the parameter along the ray where the hit occurred
    /// - <see cref="Material"/>: the material of the surface (optional)
    /// </summary>
    public class HitRecord
    {
        public Point WorldPoint;
        public Normal Normal;
        public Vec2d SurfacePoint;
        public float T;
        public Ray Ray;
        public Material? Material;

        public HitRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HitRecord"/> with position, normal, surface UV, ray parameter,
        /// ray and an optional material.
        /// </summary>
        /// <param name="worldPoint">Hit position in world-space coordinates.</param>
        /// <param name="normal">Surface normal at the hit point (world-space).</param>
        /// <param name="surfacePoint">UV texture coordinates at the hit point.</param>
        /// <param name="t">Ray parameter where the intersection occurred.</param>
        /// <param name="ray">The ray that intersected the shape.</param>
        /// <param name="material">The surface material (optional; defaults to <c>null</c>).</param>
        public HitRecord(
            Point worldPoint,
            Normal normal,
            Vec2d surfacePoint,
            float t,
            Ray ray,
            Material? material = null)
        {
            this.WorldPoint = worldPoint;
            this.Normal = normal;
            this.SurfacePoint = surfacePoint;
            this.T = t;
            this.Ray = ray;
            this.Material = material;
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
}
