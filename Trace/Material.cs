/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using System;
using System.Diagnostics;

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

    public UniformPigment()
    {
        
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
    
    public CheckeredPigment(Color color1, Color color2, int n = 10)
    {
        Color1 = color1;
        Color2 = color2;
        N = n;
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

/// <summary>
/// A textured pigment
/// The texture is given through a PFM image
/// </summary>
/// <param name="image"></param>
public class ImagePigment(HdrImage image) : Pigment
{
    public HdrImage Image = image;

    public override Color GetColor(Vec2d uv)
    {
        var col = uv.u * Image.Width;
        var row = uv.v * Image.Height;
        
        if(col >= Image.Width) col = Image.Width - 1;
        if(row >= Image.Height) row = Image.Height - 1;
        
        return Image.GetPixel((int)col, (int)row);
    }
}


/// <summary>
/// An abstract class representing a Bidirectional Reflectance Distribution Function
/// </summary>
public abstract class Brdf
{
    public Pigment Pigment = new UniformPigment(Color.White);
    
    public abstract Color Eval(Normal normal, Vec incomingDir, Vec outgoingDir, Vec2d uv);
    
    public abstract Ray ScatterRay(Pcg? pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth);
}

public class SpecularBrdf : Brdf
{
    
    public override Color Eval(Normal normal, Vec incomingDir, Vec outgoingDir, Vec2d uv)
    {
        var thetaIn = (float)Math.Acos(Vec.NormalizedDot(normal.ToVec(), incomingDir));
        var thetaOut= (float)Math.Acos(Vec.NormalizedDot(normal.ToVec(), outgoingDir));
        return Math.Abs(thetaIn - thetaOut) < 0.0001 ? Pigment.GetColor(uv) : Color.Black;
    }

    public override Ray ScatterRay(Pcg? pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth)
    {
        var rayDir = new Vec(incomingDir.X, incomingDir.Y, incomingDir.Z);
        var vecNormal = normal.ToVec();
        vecNormal.Normalize();
        var dotProd = vecNormal * rayDir;
        
        return new Ray(origin: interactionPoint,
                        direction: rayDir - 2 * vecNormal * dotProd,
                        tmin: 1e-3f,
                        tmax: float.PositiveInfinity,
                        depth: depth);
    }
}

/// <summary>
/// A class representing an ideal diffuse BRDF (also called «Lambertian»)
/// </summary>
public class DiffusiveBrdf : Brdf
{ 
    public override Color Eval(Normal normal, Vec incomingDir, Vec outgoingDir, Vec2d uv)
    {
        return Pigment.GetColor(uv) * (1.0f / (float)Math.PI);
    }

    public override Ray ScatterRay(Pcg? pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth)
    {
        var onb = Vec.CreateOnbFromZ(Normal.ToVec(normal));
        Debug.Assert(pcg != null, nameof(pcg) + " != null");
        var cosThetaSq = pcg.Random_float();
        var cosTheta = (float)Math.Sqrt(cosThetaSq);
        var sinTheta = (float)Math.Sqrt(1.0f - cosThetaSq);

        var phi = 2.0f * Math.PI * pcg.Random_float();

        return new Ray(
            origin: interactionPoint,
            direction: onb.Item1 * (float)Math.Cos(phi) * cosTheta
                       + onb.Item2 * (float)Math.Sin(phi) * cosTheta
                       + onb.Item3 * sinTheta,
            tmin: 1.0e-3f,
            tmax: float.PositiveInfinity,
            depth: depth
        );
    }
}

/// <summary>
/// Represents the material properties of a surface, including its <see cref="Pigment"/> (base color/texture) and <see cref="Brdf"/>. 
/// </summary>
public class Material
{
    public Pigment EmittedRadiance = new UniformPigment();
    public Brdf Brdf = new DiffusiveBrdf();
    
}