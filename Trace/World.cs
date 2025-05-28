namespace Trace;

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

        foreach (var intersection in Shapes.Select(shape => shape.RayIntersection(ray)).OfType<HitRecord>().Where(intersection => closest == null || intersection.T < closest.T))
        {
            closest = intersection;
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
