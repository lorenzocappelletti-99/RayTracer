/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using System.Runtime.InteropServices.Swift;

namespace Trace;

public abstract class Shape(Transformation transformation)
{
    public Transformation Transform { get; } = transformation;

    public abstract HitRecord RayIntersection(Ray ray);
}


/*class Shape:
    def __init__(self, transformation=Transformation()):
        self.transformation = transformation

    def ray_intersection(self, ray: Ray) -> Optional[HitRecord]:
        return NotImplementedError(
            "Shape.ray_intersection is an abstract method and cannot be called directly"
        )*/