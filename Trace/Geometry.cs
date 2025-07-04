/*===========================================================
 |                     Raytracer Project                    |
 |             Released under EUPL-1.2 License              |
 |                       See LICENSE                        |
 ===========================================================*/

namespace Trace;

    /***********************************************************
     *                                                         *
     *                      VEC                                *
     *                                                         *
     ***********************************************************/
public struct Vec (float x, float y, float z)
{
    public  float X = x;
    public  float Y = y;
    public  float Z = z;
    
    /// <summary>
    /// Unit vector along the X axis (1, 0, 0).
    /// </summary>
    public static readonly Vec VEC_X = new Vec(1f, 0f, 0f);
    /// <summary>
    /// Unit vector along the Y axis (0, 1, 0).
    /// </summary>
    public static readonly Vec VEC_Y = new Vec(0f, 1f, 0f);
    /// <summary>
    /// Unit vector along the Z axis (0, 0, 1).
    /// </summary>
    public static readonly Vec VEC_Z = new Vec(0f, 0f, 1f);

    public override string ToString()
     {
        return $"Vector: ({X}, {Y}, {Z})";
    }

    public bool IsClose(Vec v, float sigma = 1e-5f)
    {
        return Math.Abs(this.X - v.X) <= sigma &&
               Math.Abs(this.Y - v.Y) <= sigma &&
               Math.Abs(this.Z - v.Z) <= sigma;
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
    /// <returns></returns>
    public float SqNorm() => this.X * this.X + this.Y * this.Y + this.Z * this.Z;

    /// <summary>
    /// Calculates the norm ||v|| of a Vec type
    /// </summary>
    /// <returns></returns>
    public float Norm() => MathF.Sqrt(this.SqNorm());

    
    /// <summary>
    /// Normalizes the Vec
    /// </summary>
    /// <returns></returns>
    public void Normalize()
    {
        float norm = this.Norm(); 
        if (norm != 0)
        {
            this.X /= norm;
            this.Y /= norm;
            this.Z /= norm;
        }
    }
    
    /// <summary>
    /// Converts Vec type to Normal type.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Normal to_normal(Vec v) => new Normal(v.X, v.Y, v.Z);

    /// <summary>
    /// Rotates the current vector around the X-axis by the specified angle in degrees.
    /// </summary>
    /// <param name="angleDeg">The angle of rotation in degrees.</param>
    /// <returns>The rotated vector.</returns>
    public Vec RotationX(float angleDeg)
    {
        var t = Transformation.RotationX(angleDeg);
        return t * this;
    }

    /// <summary>
    /// Rotates the current vector around the Y-axis by the specified angle in degrees.
    /// </summary>
    /// <param name="angleDeg">The angle of rotation in degrees.</param>
    /// <returns>The rotated vector.</returns>
    public Vec RotationY(float angleDeg)
    {
        var t = Transformation.RotationY(angleDeg);
        return t * this;
    }

    /// <summary>
    /// Rotates the current vector around the Z-axis by the specified angle in degrees.
    /// </summary>
    /// <param name="angleDeg">The angle of rotation in degrees.</param>
    /// <returns>The rotated vector.</returns>
    public Vec RotationZ(float angleDeg)
    {
        var t = Transformation.RotationZ(angleDeg);
        return t * this;
    }

    /// <summary>
    /// Applies a scaling transformation to the current vector using the specified scaling vector.
    /// </summary>
    /// <param name="v">The scaling vector.</param>
    /// <returns>The scaled vector.</returns>
    public Vec Scale(Vec v)
    {
        var t = Transformation.Scaling(v);
        return t * this;
    }

    /// <summary>
    /// Normalizes Vector and transforms it into a type Normal
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public Normal ToNorm(Vec v)
    {
        v.Normalize();
        return new Normal(v.X, v.Y, v.Z);
    }

    /// <summary>
    /// Normalizes this Vector and transforms it into a type Normal
    /// </summary>
    /// <returns></returns>
    public Normal ToNorm()
    {
        Normalize();
        return new Normal(X, Y, Z);
    }


    /// <summary>
    /// Create an orthonormal basis (ONB) from a vector representing the z axis (normalized)
    /// Return a tuple containing the three vectors (e1, e2, e3) of the basis. The result is such
    /// that e3 = normal.
    /// The vector must be *normalized*, otherwise this method won't work.
    /// </summary>
    /// <param name="norm"></param>
    /// <returns></returns>
    public static (Vec, Vec, Vec) CreateOnbFromZ(Vec norm)
    {
        norm.Normalize();
        
        var sign = MathF.CopySign(1f, norm.Z);
        var a = -1.0f / (sign + norm.Z);
        var b = norm.X * norm.Y * a;
        var e1 = new Vec(1.0f + sign * norm.X * norm.X * a, sign * b, -sign * norm.X);
        var e2 = new Vec(b, sign + norm.Y * norm.Y * a, -norm.Y);
        
        return (e1, e2, new Vec(norm.X, norm.Y, norm.Z));
    }
    
    /// <summary>
    /// Applly dot product to two arguments after having
    /// normalized them
    /// Result is cosine of angle between the two vectors
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static float NormalizedDot(Vec v1, Vec v2)
    {
        var mag1 = v1.SqNorm();
        var mag2 = v2.SqNorm();
        if (mag1 == 0 || mag2 == 0) return 0;
        return (v1 * v2) / (mag1 * mag2);
    }


    
}

    /***********************************************************
     *                                                         *
     *                      POINT                              *
     *                                                         *
     ***********************************************************/
public struct Point(float x, float y, float z)
{
    public readonly float X = x;
    public readonly float Y = y;
    public readonly float Z = z;
    
    public override string ToString()
    {
        return $"Point: ({X}, {Y}, {Z})";
    }
    
    public bool IsClose(Point p, float sigma = 1e-5f)
    {
        return Math.Abs(this.X - p.X) <= sigma &&
               Math.Abs(this.Y - p.Y) <= sigma &&
               Math.Abs(this.Z - p.Z) <= sigma;
    }

    public bool AreClose(float x, float y, float sigma = 1e-5f)
    {
        return Math.Abs(x - y) <= sigma;
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
    /// <returns></returns>
    public Vec to_vec() => new Vec(this.X, this.Y, this.Z);    
    
    /// <summary>
    /// Applies a translation transformation to the current point using the specified translation vector.
    /// </summary>
    /// <param name="v">The translation vector.</param>
    /// <returns>The translated point.</returns>
    public Point Translation(Vec v)
    {
        var t = Transformation.Translation(v);
        return t * this;
    }

}

    /***********************************************************
     *                                                         *
     *                      NORMAL                             *
     *                                                         *
     ***********************************************************/
public struct Normal(float x, float y, float z)
{
    public float X = x;
    public float Y = y;
    public float Z = z;

    public override string ToString()
    {
        return $"Vec: ({X}, {Y}, {Z})";
    }
    
    public bool IsClose(Normal v, float sigma = 1e-5f)
    {
        return Math.Abs(this.X - v.X) <= sigma &&
               Math.Abs(this.Y - v.Y) <= sigma &&
               Math.Abs(this.Z - v.Z) <= sigma;
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
    
    /// <summary>
    /// Rotates the current normal around the X-axis by the specified angle in degrees.
    /// </summary>
    /// <param name="angleDeg">The angle of rotation in degrees.</param>
    /// <returns>The rotated normal.</returns>
    public Normal RotationX(float angleDeg)
    {
        var t = Transformation.RotationX(angleDeg);
        return t * this;
    }

    /// <summary>
    /// Rotates the current normal around the Y-axis by the specified angle in degrees.
    /// </summary>
    /// <param name="angleDeg">The angle of rotation in degrees.</param>
    /// <returns>The rotated normal.</returns>
    public Normal RotationY(float angleDeg)
    {
        var t = Transformation.RotationY(angleDeg);
        return t * this;
    }

    /// <summary>
    /// Rotates the current normal around the Z-axis by the specified angle in degrees.
    /// </summary>
    /// <param name="angleDeg">The angle of rotation in degrees.</param>
    /// <returns>The rotated normal.</returns>
    public Normal RotationZ(float angleDeg)
    {
        var t = Transformation.RotationZ(angleDeg);
        return t * this;
    }

    /// <summary>
    /// Scales the current normal using the specified scaling vector.
    /// </summary>
    /// <param name="v">The scaling vector.</param>
    /// <returns>The scaled normal.</returns>
    public Normal Scale(Vec v)
    {
        var t = Transformation.Scaling(v);
        return t * this;
    }

    /// <summary>
    /// Transforms normal into a type Vec
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Vec ToVec(Normal n)
    {
        return new Vec(n.X, n.Y, n.Z);
    }

    /// <summary>
    /// Transforms this normal into a type Vec
    /// </summary>
    /// <returns></returns>
    public Vec ToVec()
    {
        return new Vec(X, Y, Z);
    }

}


/// <summary>
/// Simple 2D vector for UV coordinates.
/// </summary>
public struct Vec2d
{
    public float u;
    public float v;

    public Vec2d(float u, float v)
    {
        this.u = u;
        this.v = v;
    }

    /// <summary>
    /// Checks whether this Vec2d and another are approximately equal.
    /// </summary>
    /// <param name="other">The other Vec2d to compare against.</param>
    /// <param name="epsilon">Tolerance for comparisons.</param>
    /// <returns>True if both u and v differ by less than epsilon.</returns>
    public bool IsClose(Vec2d other, float epsilon = 1e-5f)
    {
        return MathF.Abs(this.u - other.u) <= epsilon
               && MathF.Abs(this.v - other.v) <= epsilon;
    }


    
}
