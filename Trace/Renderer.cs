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
    public Color OnColor;
    
    public OnOffRenderer(World world, Color onColor) : base(world) { OnColor = onColor; }
    
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
