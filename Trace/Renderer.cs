using System.IO.Pipes;

namespace Trace;


public abstract class Renderer
{
    public World World;
    public Color BackgroundColor;
    public Ray Ray;

    protected Renderer(World world, Color backgroundColor)
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


public class FlatRenderer : Renderer{
    
    public FlatRenderer(World world, Color backgroundColor) : base(world, backgroundColor){}
    public FlatRenderer(World world) : base(world)
    {
    }


    public override Color Render(Ray ray){
        var hit = World.ray_intersection(ray);
        if(hit == null) return BackgroundColor;
        var material = hit.Material!;
        var pigmentColor = material.Pigment.GetColor(hit.SurfacePoint); 
        return pigmentColor;

    }
}

public class PathTracer : Renderer
{
    public Pcg Pgc;
    public int NumOfRays;
    public int MaxDepth;
    public int RussianRouletteLimit;
    
    public PathTracer(World world, Color backgroundColor, Pcg pgc, int numOfRays,
        int maxDepth, int russianRouletteLimit) : base(world, backgroundColor)
    {
        Pgc = pgc;
        NumOfRays = numOfRays;
        MaxDepth = maxDepth;
        RussianRouletteLimit = russianRouletteLimit;
        
    }
    
    public PathTracer(World world, Pcg pgc, int numOfRays,
        int maxDepth, int russianRouletteLimit) : base(world)
    {
        Pgc = pgc;
        NumOfRays = numOfRays;
        MaxDepth = maxDepth;
        RussianRouletteLimit = russianRouletteLimit;
        
    }
    

    public override Color Render(Ray ray)
    {
        if(Ray.Depth > MaxDepth) return Color.Black;
        
        var hitRecord = World.ray_intersection(ray);
        if(hitRecord == null) return BackgroundColor;
        
        var hitMaterial = hitRecord.Material!;
        var hitColor = hitMaterial.Pigment.GetColor(hitRecord.SurfacePoint);
        var emittedRadiance = hitMaterial.Brdf.Pigment.GetColor(hitRecord.SurfacePoint);
        
        var hitColorLum = Math.Max(hitColor.R, Math.Max(hitColor.G, hitColor.B));
        
        //Russian Roulette
        if (ray.Depth >= RussianRouletteLimit)
        {
            var q = Math.Max(0.05f, 1 - hitColorLum);
            if (this.Pgc.Random() > q) hitColor *= 1.0f / (1.0f - q);
            else return emittedRadiance;
        }

        var cumRadiance = new Color(0.0f, 0.0f, 0.0f);
        if (hitColorLum > 0.0f)
        {
            for (var rayIndex=0; rayIndex < NumOfRays; rayIndex++)
            {
                var newRay = hitMaterial.Brdf.ScatterRay(
                    pcg: this.Pgc,
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

