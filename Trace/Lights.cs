namespace Trace;

/// <summary>
/// A point light (used by the point-light renderer)
/// This class holds information about a point light (a Dirac's delta in the rendering equation). The class has
/// the following fields:
///   `Position`: a `Point` object holding the position of the point light in 3D space
///   `Color`: a `Color` object holding information about the color of the point light 
///   `LinearRadius`: a floating-point number. If non-zero, this «linear radius» `r` is used to compute the solid
///     angle subtended by the light at a given distance `d` through the formula `(r / d)*(r / d)`.
///
    /// </summary>
    public class PointLight
    {
        public Point Position;
        public Color Color;
        public float LinearRadius;

        public PointLight(Point position, Color color)
        {
            Position = position;
            Color = color;
        }
        public PointLight(Point position, Color color, float linearRadius)
        {
            Position = position;
            Color = color;
            LinearRadius = linearRadius;
        }
    }