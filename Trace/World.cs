namespace Trace;

/// <summary>
/// Represents the scene to be rendered.
/// - <see cref="Shapes"/>: list of objects in the scene,
/// - <see cref="PointLights"/>: list of PointLights in the scene
/// </summary>
public class World
{
    public List<Shape> Shapes { get; set; } = [];
    public List<PointLight> PointLights { get; set; } = [];


    public void AddShape(Shape shape)
    {
        Shapes.Add(shape);
    }

    public void AddLight(PointLight light)
    {
        PointLights.Add(light);
    }

    
    /// <summary>
    ///  returns the HitRecord of the closest shape to param "ray" origin.
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public HitRecord? ray_intersection(Ray ray)
    {
        HitRecord? closest = null;
        float bestT = float.PositiveInfinity;

        foreach (var shape in Shapes)
        {
            if (shape.RayIntersection(ray) is { } hit && hit.T < bestT)
            {
                closest = hit;
                bestT = hit.T;
            }
        }
        return closest;
    }
    
    public bool IsPointVisible(Point p, Point observerPoint)
    {
        var dir = p - observerPoint;
        var dirNorm = dir.Norm();

        var ray = new Ray(origin: observerPoint, direction: dir, tmin: 1e-2f / dirNorm, tmax:1.0f);
        return Shapes.All(shape => !shape.QuickRayIntersection(ray));
    }
}
