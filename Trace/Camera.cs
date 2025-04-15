namespace Trace;

public interface ICamera
{

    /*
     def fire_ray(self, u, v):
        """Fire a ray through the camera.

        This is an abstract method. You should redefine it in derived classes.

        Fire a ray that goes through the screen at the position (u, v). The exact meaning
        of these coordinates depend on the projection used by the camera.
        """
        raise NotImplementedError(f"Camera.fire_ray(u={u}, v={v}) is not implemented")
     */
}

public class OrthogonalProjection : ICamera
{
    
    /*
     def __init__(self, aspect_ratio=1.0, transformation=Transformation()):
        self.aspect_ratio = aspect_ratio
        self.transformation = transformation

    def fire_ray(self, u: float, v: float) -> Ray:
        origin = Point(-1.0, (1.0 - 2 * u) * self.aspect_ratio, 2 * v - 1)
        direction = VEC_X
        return Ray(origin=origin, dir=direction, tmin=1.0e-5).transform(self.transformation)
     */

}

public class PerspectiveProjection : ICamera
{
/*
 def __init__(self, distance=1.0, aspect_ratio=1.0, transformation=Transformation()):
        self.distance = distance
        self.aspect_ratio = aspect_ratio
        self.transformation = transformation

    def fire_ray(self, u: float, v: float) -> Ray:
        origin = Point(-self.distance, 0.0, 0.0)
        direction = Vec(self.distance, (1.0 - 2 * u) * self.aspect_ratio, 2 * v - 1)
        return Ray(origin=origin, dir=direction, tmin=1.0e-5).transform(self.transformation)
 */
}
