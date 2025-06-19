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
    
    public static void DrawProgressBar(int percent, int barSize = 50)
    {
        var progress = (int)((percent / 100.0) * barSize);
        var bar = new string('>', progress).PadRight(barSize, '-');
        Console.Write($"\r[{bar}] {percent}%");
    }
    
    public static void PrintTime(TimeSpan elapsedWallClockTime)
    {
        switch (elapsedWallClockTime.TotalSeconds)
        {
            case <= 60:
                Console.WriteLine($"\nWall-Clock Time Elapsed for Firing all Rays:{elapsedWallClockTime.TotalSeconds} seconds");
                break;
            case > 60:
                Console.WriteLine($"\nWall-Clock Time Elapsed for Firing all Rays:{elapsedWallClockTime.TotalMinutes} minutes");
                break;
        }
    }


}

