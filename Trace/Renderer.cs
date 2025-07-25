using System.Diagnostics;

namespace Trace;

/// <summary>
/// Base class for all renderers.
/// </summary>
public abstract class Renderer
{
    public World World;
    public Color BackgroundColor;
    public Ray Ray;
    
    public Renderer(World world, Color backgroundColor)
    {
        World = world;
        BackgroundColor = backgroundColor;
    }

    protected Renderer(World world)
    {
        World = world;
        BackgroundColor = Color.Black;
    }

    public abstract Color Render(Ray ray);
}

/// <summary>
/// This renderer returns a fixed color if the ray hits anything; otherwise background.
/// Useful for debugging geometry visibility.
/// </summary>
public class OnOffRenderer : Renderer
{
    public Color OnColor = Color.White;
    
    public OnOffRenderer(World world, Color onColor) : base(world) { OnColor = onColor; }
    
    public OnOffRenderer(World world) : base(world) {}
    
    public OnOffRenderer(World world, Color backgroundColor, Color onColor) : 
        base(world, backgroundColor) 
            { OnColor = onColor; }

    public override Color Render(Ray ray)
    {
        return World.ray_intersection(ray) != null ? OnColor : BackgroundColor;
    }
}


/// <summary>
/// This renderer returns the emissive color of surfaces intersected by the ray.
/// Ignores lighting and reflections. Useful for testing materials' emissions.
/// </summary>
public class FlatRenderer : Renderer{
    
    public FlatRenderer(World world, Color backgroundColor) : base(world, backgroundColor){}
    public FlatRenderer(World world) : base(world)
    {
    }


    public override Color Render(Ray ray){
        var hit = World.ray_intersection(ray);
        if(hit == null) return BackgroundColor;
        Debug.Assert(hit.Material != null);
        var material = hit.Material;
        var pigmentColor = material.EmittedRadiance.GetColor(hit.SurfacePoint);
        return pigmentColor;
    }
}

/// <summary>
/// Path tracer renderer with multiple rays per intersection for global illumination.
/// </summary>
public class PathTracer : Renderer
{
    public Pcg? Pgc { get; }
    public int NumOfRays { get; }
    public int MaxDepth { get; }
    public int RussianRouletteLimit { get; }

    public PathTracer(
        World world,
        Color background = default, 
        Pcg? pcg = null,
        int numOfRays = 10,
        int maxDepth = 10,
        int russianRouletteLimit = 3
    ) : base(world, background == default ? Color.Black : background)
    {
        Pgc = pcg ?? new Pcg();
        NumOfRays = numOfRays;
        MaxDepth = maxDepth;
        RussianRouletteLimit = russianRouletteLimit;
    }

    public override Color Render(Ray ray)
    {
        if(ray.Depth > MaxDepth) return Color.Black;
        
        var hitRecord = World.ray_intersection(ray);
        if(hitRecord == null) return BackgroundColor;

        Debug.Assert(hitRecord.Material != null);
        var hitMaterial = hitRecord.Material;
        var hitColor = hitMaterial.Brdf.Pigment.GetColor(hitRecord.SurfacePoint);
        var emittedRadiance = hitMaterial.EmittedRadiance.GetColor(hitRecord.SurfacePoint);
        
        var hitColorLum = Math.Max(hitColor.R, Math.Max(hitColor.G, hitColor.B));
        
        //Russian Roulette
        if (ray.Depth >= RussianRouletteLimit)
        {
            //threshold favourites bright paths! 0.05 is to ensure q doesn't go to zero.
            var q = Math.Max(0.05f, 1 - hitColorLum);
            if (Pgc != null && Pgc.Random_float() > q) 
                hitColor *= 1.0f / (1.0f - q);
            else return emittedRadiance;
        }

        var cumRadiance = new Color(0.0f, 0.0f, 0.0f);
        if (hitColorLum > 0.0f)
        {
            for (var rayIndex=0; rayIndex < NumOfRays; rayIndex++)
            {
                var newRay = hitMaterial.Brdf.ScatterRay(
                    pcg: Pgc,
                    incomingDir: hitRecord.Ray.Direction,
                    interactionPoint: hitRecord.WorldPoint,
                    normal: hitRecord.Normal,
                    depth: ray.Depth + 1
                );
                var newRadiance = Render(newRay);
                cumRadiance += hitColor * newRadiance;
            }
        }
        return emittedRadiance + cumRadiance * (1.0f / NumOfRays);
    }
}


/// <summary>
/// Renderer that simulates direct illumination from point lights with ambient term.
/// Useful for real-time previews or stylized renders.
/// </summary>
public class PointLightRenderer : Renderer
{
       public Color AmbientColor = new Color(0.1f, 0.1f, 0.1f);
       public PointLightRenderer(World world, Color backgroundColor) : base(world, backgroundColor){}
       
       public PointLightRenderer(World world, Color backgroundColor, Color ambientColor) : base(world, backgroundColor)
       {
           AmbientColor = ambientColor;
       }
       public override Color Render(Ray ray)
       {
           var hitRecord = World.ray_intersection(ray);
           if(hitRecord == null) return BackgroundColor;

           Debug.Assert(hitRecord.Material != null);
           var hitMaterial = hitRecord.Material;
           
           var resultColor = AmbientColor;
           foreach (var curLight in World.PointLights)
           {
               if (World.IsPointVisible(curLight.Position, hitRecord.WorldPoint))
               {
                   var distanceVec = hitRecord.WorldPoint - curLight.Position;
                   var distance = distanceVec.Norm();
                   var inDir = distanceVec * (1.0f / distance);
                   var cosTheta = Math.Max(0, Vec.NormalizedDot(-inDir, hitRecord.Normal.ToVec()));
                   var distanceFactor = (curLight.LinearRadius > 0) ? 
                       (float)Math.Pow(curLight.LinearRadius / distance, 2) : 
                       1.0f;

                   var emittedColor = hitMaterial.EmittedRadiance.GetColor(hitRecord.SurfacePoint);
                   var brdfColor = hitMaterial.Brdf.Eval(
                       normal: hitRecord.Normal,
                       incomingDir: inDir,
                       outgoingDir: -ray.Direction,
                       uv: hitRecord.SurfacePoint);
                   resultColor += (emittedColor + brdfColor) * curLight.Color * cosTheta * distanceFactor;
               }
           }
           return resultColor;
       }
        
}

