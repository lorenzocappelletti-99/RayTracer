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
        /*
         HitRecord? closest = null;

        foreach (var t in Shapes)
        {
            var intersection = t.RayIntersection(ray);

            if (intersection != null)
            {
                closest = intersection;
                break;
            }
        }

        return closest;
         */
    }

}
