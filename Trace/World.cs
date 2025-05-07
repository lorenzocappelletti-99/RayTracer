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
        return Shapes.Select(t => t.RayIntersection(ray)).OfType<HitRecord>().FirstOrDefault();
        
         /*HitRecord? closest = null;

         
        foreach (var shape in Shapes)
        {
            var intersection = shape.RayIntersection(ray);

            if (intersection == null) continue;
            closest = intersection;
            break;
        }

        return closest;
        */
    }

    /*
    public bool IsPointVisible(Point p, Point observerPoint)
    {
        var dir = p - observerPoint;
        var normDir = dir.Normalize();

        var ray = new Ray(origin: observerPoint, direction: dir);
        foreach (var shape in Shapes)
        {
            if (shape.QuickRayIntersection(ray) != null) return false;

        }
        return true;
    }

    def is_point_visible(self, point: Point, observer_pos: Point):
    direction = point - observer_pos
        dir_norm = direction.norm()

    ray = Ray(origin=observer_pos, dir=direction, tmin=1e-2 / dir_norm, tmax=1.0)
    for shape in self.shapes:
    if shape.quick_ray_intersection(ray):
    return False

    return True
    */

}
