namespace Trace;


public abstract class Renderer
{
    public World World;
    public Color BackgroundColor = Color.Black;
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
    public Color OnColor;
    
    public OnOffRenderer(World world, Color onColor) : base(world) { OnColor = onColor; }

    public override Color Render(Ray ray)
    {
        return World.ray_intersection(ray) != null ? Color.White : Color.Black;
    }
}


public class FlatRenderer : Renderer{
    
    public FlatRenderer(World world, Color backgroundColor) : base(world, backgroundColor){}
    
    public override Color Render(Ray ray){
        var hit = World.ray_intersection(ray);
        if(hit == null) return BackgroundColor;
        var material = hit.Material;
        return (material.Brdf.Pigment.GetColor(hit.SurfacePoint));
        //return (material.Brdf.Pigment.GetColor(hit.SurfacePoint)) + material.EmittedRadiance.GetColor(hit.SurfacePoint));
    }
}
