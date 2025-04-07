/*===========================================================
 |                     Raytracer Project                    |
 |             Released under EUPL-1.2 License              |
 |                       See LICENSE                        |
 ===========================================================*/

namespace Trace;

public struct Vec (float x, float y, float z)
{
    public readonly float X = x;
    public readonly float Y = y;
    public readonly float Z = z;

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
    
    /// <summary>
    /// Sum of two Vec types
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Vec operator +(Vec v1, Vec v2) => new Vec(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    
    /// <summary>
    /// Difference of two Vec types
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Vec operator -(Vec v1, Vec v2) => new Vec(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    
    /// <summary>
    /// Product between a Vec type and a scalar 
    /// </summary>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vec operator *(float s, Vec v) => new Vec(v.X * s, v.Y * s, v.Z * s);

    /// <summary>
    /// Product between scalar and Vec type
    /// </summary>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vec operator *(Vec v, float s) => new Vec(v.X * s, v.Y * s, v.Z * s);
    
    /// <summary>
    /// Division between a Vec type and a scalar
    /// </summary>
    /// <param name="v"></param>
    /// <param name="s"></param>
    /// <returns></returns>
    /// <exception cref="DivideByZeroException"></exception>
    public static Vec operator /(Vec v, float s)
    {
        if (s == 0)
            throw new DivideByZeroException("Cannot divide a vector by zero.");

        return new Vec(v.X / s, v.Y / s, v.Z / s);
    }
    
    /// <summary>
    /// Inverse of a Vec type (changes sign of entries)
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Vec operator -(Vec v) => new Vec(-v.X, -v.Y, -v.Z);
    
    /// <summary>
    /// Dot product between two Vec types
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static float operator *(Vec v1, Vec v2) => v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
    
    /// <summary>
    /// Cross product between two vectors types
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
    
    /// <summary>
    /// Calculates the squared norm ||v||^2 of a Vec type
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static float SqNorm(Vec v) => v.X * v.X + v.Y * v.Y + v.Z * v.Z;

    /// <summary>
    /// Calculates the norm ||v||^2 of a Vec type
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static float Norm(Vec v) => MathF.Sqrt(SqNorm(v));

    
    /// <summary>
    /// Normalizes the Vec
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
    public readonly float X = x;
    public readonly float Y = y;
    public readonly float Z = z;
    
    public override string ToString()
    {
        return $"Point: ({X}, {Y}, {Z})";
    }
    
    public static bool AreClose(Point p1, Point p2, float sigma = 1e-5f)
    {
        return Math.Abs(p1.X - p2.X) <= sigma &&
               Math.Abs(p1.Y - p2.Y) <= sigma &&
               Math.Abs(p1.Z - p2.Z) <= sigma;
    }

    /// <summary>
    /// Sum of a Point type with Vec type. Returns a Point type.
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Point operator +(Point p1, Vec v2) => new Point(p1.X + v2.X, p1.Y + v2.Y, p1.Z + v2.Z);
    
    /// <summary>
    /// Difference of a Point type with Vec type. Returns a Point type.
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Point operator -(Point p1, Vec v2) => new Point(p1.X - v2.X, p1.Y - v2.Y, p1.Z - v2.Z);

    /// <summary>
    /// Difference of two Points type. Returns a Vec type.
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static Vec operator -(Point p1, Point p2) => new Vec(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);


    /// <summary>
    /// Converts Point type to Vec type.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static Vec to_vec(Point p) => new Vec(p.X, p.Y, p.Z);
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
    
    public static bool AreClose(Normal n1, Normal n2, float sigma = 1e-5f)
    {
        return Math.Abs(n1.X - n2.X) <= sigma &&
               Math.Abs(n1.Y - n2.Y) <= sigma &&
               Math.Abs(n1.Z - n2.Z) <= sigma;
    }

        
    /// <summary>
    /// Converts Normal type to Vec type.
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Vec to_vec(Normal n) => new Vec(n.X, n.Y, n.Z);
    
    /// <summary>
    /// Product between a Normal type and a scalar 
    /// </summary>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Normal operator *(float s, Normal v) => new Normal(v.X * s, v.Y * s, v.Z * s);

    /// <summary>
    /// Product between scalar and Vec type
    /// </summary>
    /// <param name="s"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Normal operator *(Normal v, float s) => new Normal(v.X * s, v.Y * s, v.Z * s);
    
    /// <summary>
    /// Division between a Normal type and a scalar
    /// </summary>
    /// <param name="n"></param>
    /// <param name="s"></param>
    /// <returns></returns>
    /// <exception cref="DivideByZeroException"></exception>
    public static Normal operator /(Normal n, float s)
    {
        if (s == 0)
            throw new DivideByZeroException("Cannot divide a vector by zero.");

        return new Normal(n.X / s, n.Y / s, n.Z / s);
    }
    
    /// <summary>
    /// Inverse of a Normal type (changes sign of entries)
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Normal operator -(Normal v) => new Normal(-v.X, -v.Y, -v.Z);
    
    /// <summary>
    /// Dot product between Vec types and Normal type
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    public static float operator *(Vec v1, Normal n2) => v1.X * n2.X + v1.Y * n2.Y + v1.Z * n2.Z;
    
    /// <summary>
    /// Dot product between Normal types and Vec type
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    public static float operator *(Normal v1, Vec n2) => v1.X * n2.X + v1.Y * n2.Y + v1.Z * n2.Z;

    /// <summary>
    /// Cross product between two Normal types
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    public static Vec operator %(Normal n1, Normal n2)
    {
        return new Vec(
            n1.Y * n2.Z - n1.Z * n2.Y,
            n1.Z * n2.X - n1.X * n2.Z,
            n1.X * n2.Y - n1.Y * n2.X
        );
    }
    
    /// <summary>
    /// Cross product between Normal type and Vec type.
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    public static Vec operator %(Normal n1, Vec n2)
    {
        return new Vec(
            n1.Y * n2.Z - n1.Z * n2.Y,
            n1.Z * n2.X - n1.X * n2.Z,
            n1.X * n2.Y - n1.Y * n2.X
        );
    }
    
    /// <summary>
    /// Cross product between Vec type and Normal type.
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    public static Vec operator %(Vec n1, Normal n2)
    {
        return new Vec(
            n1.Y * n2.Z - n1.Z * n2.Y,
            n1.Z * n2.X - n1.X * n2.Z,
            n1.X * n2.Y - n1.Y * n2.X
        );
    }
    
    
    /// <summary>
    /// Calculates the squared norm ||v||^2 of a Vec type
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static float SqNorm(Normal v) => v.X * v.X + v.Y * v.Y + v.Z * v.Z;
    

    /// <summary>
    /// Calculates the norm ||v||^2 of a Vec type
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static float Norm(Normal v) => MathF.Sqrt(SqNorm(v));

    
    /// <summary>
    /// Normalizes the Normal
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public void Normalize()
    {
        float norm = Norm(this); // or Norm(this) if it's static
        if (norm != 0)
        {
            this.X /= norm;
            this.Y /= norm;
            this.Z /= norm;
        }
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
