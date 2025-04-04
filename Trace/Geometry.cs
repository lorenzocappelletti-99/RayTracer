/*===========================================================
 |                     Raytracer Project                    |
 |             Released under EUPL-1.2 License              |
 |                       See LICENSE                        |
 ===========================================================*/

namespace Trace;

public struct Vec (float x, float y, float z)
{
    public float X = x;
    public float Y = y;
    public float Z = z;

    public override string ToString()
    {
        return $"Vector: ({X}, {Y}, {Z})";
    }

    public static bool AreClose(Vec v1, Vec v2, float sigma = 1e-5f)
    {
        return Math.Abs(v1.X - v2.X) <= sigma &&
               Math.Abs(v1.Y - v2.Y) <= sigma &&
               Math.Abs(v1.Z - v2.Z) <= sigma;
    }
    
    // Sum and difference of two vectors
    public static Vec operator +(Vec v1, Vec v2) => new Vec(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    public static Vec operator -(Vec v1, Vec v2) => new Vec(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    
    // Product between a vector and a scalar
    public static Vec operator *(float s, Vec v) => new Vec(v.X * s, v.Y * s, v.Z * s);

    public static Vec operator *(Vec v, float s) => new Vec(v.X * s, v.Y * s, v.Z * s);
    
    // Division between a vector and a scalar
    public static Vec operator /(Vec v, float s)
    {
        if (s == 0)
            throw new DivideByZeroException("Cannot divide a vector by zero.");

        return new Vec(v.X / s, v.Y / s, v.Z / s);
    }
    
    // Inverse of a vector
    public static Vec operator -(Vec v) => new Vec(-v.X, -v.Y, -v.Z);
    
    // Dot product between two vectors
    public static float operator *(Vec v1, Vec v2) => v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
    
    /// <summary>
    /// Compute the cross product between two vectors
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Vec operator %(Vec v1, Vec v2)
    {
        return new Vec(
            v1.Y * v2.Z - v1.Z * v2.Y,
            v1.Z * v2.X - v1.X * v2.Z,
            v1.X * v2.Y - v1.Y * v2.X
        );
    }
    
    // Calculate ||v|| and ||v||^2
    public static float SqNorm(Vec v) => v.X * v.X + v.Y * v.Y + v.Z * v.Z;

    /// <summary>
    /// Calculates the norm of the vector
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static float Norm(Vec v) => MathF.Sqrt(SqNorm(v));

    
    /// <summary>
    /// Normalizes the vector
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vec Normalize(Vec v)
    {
        var v1 = v / Norm(v);
        return v1;
    }
    
    
}

public struct Point(float x, float y, float z)
{
    public float X = x;
    public float Y = y;
    public float Z = z;
    
    public override string ToString()
    {
        return $"Point: ({X}, {Y}, {Z})";
    }
}

public struct Normal(float x, float y, float z)
{
    public float X = x;
    public float Y = y;
    public float Z = z;

    public override string ToString()
    {
        return $"Vector: ({X}, {Y}, {Z})";
    }
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
