/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using Xunit;
namespace Trace.Tests;

public class RandomTests
{
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
}