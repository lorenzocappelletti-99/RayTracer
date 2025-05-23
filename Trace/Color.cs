/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

namespace Trace;
/// <summary>
/// Represents an RGB color with floating-point components.
/// </summary>
public struct Color(float r, float g, float b)
{
    public float R = r, G = g, B = b;
    
    // Common colors
    public static readonly Color Black = new Color(0f, 0f, 0f);
    public static readonly Color White = new Color(1f, 1f, 1f);
    public static readonly Color Red   = new Color(1f,   0f,   0f);
    public static readonly Color Green = new Color(0f,   1f,   0f);
    public static readonly Color Blue  = new Color(0f,   0f,   1f);
    public static readonly Color Yellow = new Color(1f,   1f,   0f);
    public static readonly Color Purple  = new Color(0.5f, 0f,   0.5f);

    

    // Add two colors
    public static Color operator +(Color c1, Color c2)
    {
        return new Color(c1.R + c2.R, c1.G + c2.G, c1.B + c2.B);
    }

    // Multiply two colors (component-wise multiplication)
    public static Color operator *(Color c1, Color c2)
    {
        return new Color(c1.R * c2.R, c1.G * c2.G, c1.B * c2.B);
    }

    // Multiply color by scalar
    public static Color operator *(Color c, float scalar)
    {
        return new Color(c.R * scalar, c.G * scalar, c.B * scalar);
    }

    public static Color operator *(float scalar, Color c)
    {
        return c * scalar; // Reuse the existing overload
    }

    /// <summary>
    /// Verify that 2 floats are equal within a specified tolerance (epsilon)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="epsilon"></param>
    /// <returns></returns>
    public static bool are_close(float a, float b, float epsilon = 1e-5f)
    {
        return Math.Abs(a - b) <= epsilon;
    }

    public bool IsClose(Color a, float epsilon = 1e-5f)
    {
        return are_close(R, a.R) &&
               are_close(B, a.B) &&
               are_close(G, a.G);
    }

    public static bool are_close_colors(Color c1, Color c2)
    {
        return are_close(c1.R, c2.R) &&
               are_close(c1.B, c2.B) &&
               are_close(c1.G, c2.G);
    }

    public override string ToString()
    {
        return $"(R: {R}, G: {G}, B: {B})";
    }
    
    /// <summary>
    /// Calcola luminosità con la formula di Shirley & Morley
    /// </summary>
    /// <returns></returns>
    public float Luminosity()
    {
        float maxVal = Math.Max(R, Math.Max(G, B));
        float minVal = Math.Min(R, Math.Min(G, B));
        return (maxVal + minVal) / 2.0f;
    }


}