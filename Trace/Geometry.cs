namespace Trace;

public struct Vec
{
    public int X;
    public int Y;
    public int Z;
}

public struct Point
{
    public int X;
    public int Y;
    public int Z;
}

public struct Normal
{
    public int X;
    public int Y;
    public int Z;
}

public struct HowMatrix(float a, float b, float c, float d)
{
    public float A = a; // element (0,0)
    public float B = b; // element (0,1)
    public float C = c; // element (1,0)
    public float D = d; // element (1,1)

    public void Display()
    {
        Console.WriteLine($"Matrix: \n[{A} {B}]\n[{C} {D}]");
    } 
    
    public override string ToString()
    {
        return $"Matrix: \n[{A} {B}]\n[{C} {D}]";
    }
}
