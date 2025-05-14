/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

namespace Trace;

/// <summary>
/// Abstractly represents a pigment, i.e., a function mapping
/// a color to each point (u,v) of a parametric surface.
/// </summary>
public abstract class Pigment
{
    /// <summary>
    /// Returns the color of the pigment at the specified UV coordinates.
    /// </summary>
    /// <param name="uv">Parametric coordinates (u,v).</param>
    /// <returns>The corresponding color.</returns>
    public abstract Color GetColor(Vec2d uv);
}

/// <summary>
/// A uniform pigment: always returns the same color,
/// regardless of the UV coordinates.
/// </summary>
public class UniformPigment : Pigment
{
    public Color Color;

    /// <summary>
    /// Initializes a uniform pigment with the given base color.
    /// </summary>
    /// <param name="color">Base color of the pigment.</param>
    public UniformPigment(Color color)
    {
        this.Color = color;
    }

    /// <summary>
    /// Returns the uniform color, ignoring the UV coordinates.
    /// </summary>
    /// <param name="uv">Parametric coordinates (u,v) (ignored).</param>
    /// <returns>The uniform color.</returns>
    public override Color GetColor(Vec2d uv)
    {
        return Color;
    }
}

public class CheckeredPigment : Pigment
{
    public Color Color1;
    public Color Color2;
    public int N; 
    
    public CheckeredPigment(Color color1, Color color2, int n = 20)
    {
        this.Color1 = color1;
        this.Color2 = color2;
        this.N = n;
    }
    
    /// <summary>
    /// Returns one of the two colors depending on the checker position at UV.
    /// </summary>
    /// <param name="uv">Parametric coordinates (u,v).</param>
    /// <returns>Color1 or Color2 based on the checker parity.</returns>
    public override Color GetColor(Vec2d uv)
    {
        int intU = (int)Math.Floor(uv.u * N);
        int intV = (int)Math.Floor(uv.v * N);
        bool even = (intU % 2) == (intV % 2);
        return even ? Color1 : Color2;
    }
}

