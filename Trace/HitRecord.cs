namespace Trace;


public class HitRecord
{

    public Point WorldPoint;
    public Normal Normal;
    public Vec2d SurfacePoint;
    public float t;
    public Ray Ray;

    /// <summary>
    /// TO BE IMPLEMENTED
    /// </summary>
    /// <param name="epsilon"></param>
    /// <returns></returns>
    public bool IsClose(float epsilon = 1e-5f)
    {
        return true;
    }
    
    /*    def is_close(self, other: Union["HitRecord", None], epsilon=1e-5) -> bool:
        """Check whether two `HitRecord` represent the same hit event or not"""
        if not other:
            return False

        return (
                self.world_point.is_close(other.world_point) and
                self.normal.is_close(other.normal) and
                self.surface_point.is_close(other.surface_point) and
                (abs(self.t - other.t) < epsilon) and
                self.ray.is_close(other.ray)
        )* /*/
}
