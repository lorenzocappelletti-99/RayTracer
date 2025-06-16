using System;

namespace Trace;

public static class Misc
{
    /// <summary>
    /// Verify that 2 floats are equal within a specified tolerance (epsilon)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="epsilon"></param>
    /// <returns></returns>
    public static bool AreClose(float a, float b, float epsilon = 1e-5f)
    {
        return Math.Abs(a - b) <= epsilon;
    }
}