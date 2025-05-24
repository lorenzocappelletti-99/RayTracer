/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using Xunit;
namespace Trace.Tests;

public class RandomTests
{
    
    public static bool Approx(float a, float b, float epsilon = 1e-4f)
    {
        return MathF.Abs(a - b) < epsilon;
    }

    
    [Fact]
    public void TestRandom()
    {
        var pcg = new Pcg();
        Assert.True(pcg.State == 1753877967969059832);
        Assert.True(pcg.Inc == 109);

        foreach (var expected in new uint[]
                 { 2707161783, 2068313097, 3122475824, 2211639955, 3215226955, 3421331566 })
        {
            var result = pcg.Random();
            Assert.True(expected == result);
        }
    }

    [Fact]
    public void TestOnb()
    {
        var pcg = new Pcg();

        for (int i = 0; i < 1000; i++)
        {
            Vec normal = new Vec(pcg.Random(), pcg.Random(), pcg.Random());
            normal.Normalize();
            var (e1, e2, e3) = Vec.CreateOnbFromZ(normal);
            
            Assert.True(e3.IsClose(normal));
            
            Assert.True(Approx(1f, e1.SqNorm()));
            Assert.True(Approx(1f, e2.SqNorm()));
            Assert.True(Approx(1f, e3.SqNorm()));
            
            Assert.True(Approx(0f, e1 * e2));
            Assert.True(Approx(0f, e2 * e3));
            Assert.True(Approx(0f, e3 * e1));
        }
    }
    
}