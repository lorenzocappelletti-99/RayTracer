/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

namespace Trace;

public struct Color(float r, float g, float b)
{
    public float R = r, G = g, B = b;

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

    public static bool are_close(float a, float b, float epsilon = 1e-5f)
    {
        return Math.Abs(a - b) <= epsilon;
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