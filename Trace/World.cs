namespace Trace;

public class World
{
    public List<Shape> Shapes { get; set; } = [];

    public void AddShape(Shape shape)
    {
        Shapes.Add(shape);
    }

    
    /// <summary>
    ///  returns the HitRecord of the closest shape to param "ray" origin.
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public HitRecord? ray_intersection(Ray ray)
    {
        HitRecord? closest = null;

        foreach (var intersection in Shapes.Select(shape => shape.RayIntersection(ray)).OfType<HitRecord>().Where(intersection => closest == null || intersection.t < closest.t))
        {
            closest = intersection;
        }
        return closest;

    }
    
    public bool IsPointVisible(Point p, Point observerPoint)
    {
        var dir = p - observerPoint;
        var dirNorm = dir.SqNorm();

        var ray = new Ray(origin: observerPoint, direction: dir, tmin: 1e-2f / dirNorm, tmax:1.0f);
        foreach (var shape in Shapes)
        {
            if (shape.QuickRayIntersection(ray) == true) return false;
        }
        return true;
    }
}
