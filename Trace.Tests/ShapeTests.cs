/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using Xunit;
namespace Trace.Tests;

public class ShapeTests
{
    [Fact]
    public void TestSphere()
    {
        Ray ray = new Ray(origin: new Point(0, 0, 2), direction: -Vec.VEC_Z);
        
        Sphere sphere = new Sphere();
         
        //da finire
        
    }
    
}