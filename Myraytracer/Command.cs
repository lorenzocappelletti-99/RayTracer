/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using System.Diagnostics;
using CliFx;
using CliFx.Exceptions;
using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Globalization;
using Trace;

namespace Myraytracer;

[Command("render", Description = "Generates images given an input file")]
public class RenderCommand : ICommand
{
    [CommandParameter(1, Name = "inputFile", Description = "File source with description of scene")]
    public string? InputFile { get; set; }
    [CommandOption("width", 'w', Description = "Image width")]
    public int Width { get; init; } = 1366;

    [CommandOption("height", 'h', Description = "Image height")]
    public int Height { get; init; } = 768;

    [CommandOption("output", 'o', Description = "Output file name (LDR)")]
    public string OutputLdrFileName { get; init; } = "demo.png";
    
    [CommandOption("AntiAliasing", 'A', Description = "Anti-aliasing enabled")]
    public bool AntiAliasing { get; init; } = false;
    
    [CommandOption("NumOfRays", 'R', Description = "Number of rays at each recursion")]
    public int NumOfRays { get; init; } = 3;
    
    [CommandOption("RussianRoulette", 'x', Description = "Russian roulette")]
    public int RussianRoulette { get; init; } = 1;
    
    [CommandOption("MaxDepth", 'D', Description = "Max number of reflections")]
    public int MaxDepth { get; init; } = 2;
    
    [CommandOption("Renderer", 'r', Description = "Renderer type")]
    public string Renderer { get; init; } = "PathTracer";
    
    [CommandOption("factor", 'f', Description = "Tone mapping scale factor")]
    public float Factor { get; init; } = 0.6f;
    

    public ValueTask ExecuteAsync(IConsole console)
    {
        if (InputFile == null) return default;
        InputFile = "input/" + InputFile;
        var reader = new StreamReader(InputFile);
        var inputStream = new InputStream(reader, InputFile);
        
        //PARSING
        InputStream.Scene? scene = null;
        try
        {
            console.WriteLine("Parsing scene...");
            scene = InputStream.Scene.ParseScene(inputStream);
            console.WriteLine("Scene parsed successfully!");
        }
        catch (GrammarError ex)
        {
            console.Error.WriteLine("\n--- PARSING ERROR ---");
            console.Error.WriteLine(ex.ToString());
            console.Error.WriteLine("---------------------\n");
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            console.Error.WriteLine("\n--- AN UNEXPECTED ERROR OCCURRED ---");
            console.Error.WriteLine(ex.ToString());
            console.Error.WriteLine("------------------------------------\n");
            Environment.Exit(1); 
        }
        console.Output.WriteLine($"File {InputFile} read!");
        
        console.Output.WriteLine($"Creating scene:\n width: {Width}, height: {Height}\n output: {OutputLdrFileName}\n renderer: {Renderer}, number of rays: {NumOfRays}, max depth: {MaxDepth}, russian roulette: {RussianRoulette}, antiAliasing: {AntiAliasing} \n");
        
        var image = new HdrImage(Width, Height);
        var tracer = new ImageTracer(image, scene.Camera); 

        var stopwatch = new Stopwatch();
        
        if(AntiAliasing) tracer.SamplesPerSide = 4;
        if (Renderer.Equals("PointLight", StringComparison.OrdinalIgnoreCase))
        {
            //scene!!.World.AddLight(new PointLight(new Point(-10, 0, 10f), new Color(1, 1, 1)));
            if(scene.World.PointLights.Count == 0) 
                console.Output.WriteLine("No point lights found. Needed for PointLight rendering. try: \n " +
                                  "pointlight( [vector(position)], <color>, 0.0 )");
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new PointLightRenderer(scene.World, Color.Black);
            
            stopwatch.Start();

            tracer.FireAllRays(render.Render);

            stopwatch.Stop();
            var elapsedWallClockTime = stopwatch.Elapsed;
            Misc.PrintTime(elapsedWallClockTime);
        }
        if (Renderer.Equals("PathTracer", StringComparison.OrdinalIgnoreCase))
        {
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new PathTracer(scene.World, Color.Black, new Pcg(), NumOfRays, MaxDepth, RussianRoulette);
            
            stopwatch.Start();

            tracer.FireAllRays(render.Render);

            stopwatch.Stop();
            var elapsedWallClockTime = stopwatch.Elapsed;
            Misc.PrintTime(elapsedWallClockTime);
        }
        if (Renderer.Equals("FlatRender", StringComparison.OrdinalIgnoreCase))
        {
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new FlatRenderer(scene.World, Color.Black);
                
            stopwatch.Start();

            tracer.FireAllRays(render.Render);

            stopwatch.Stop();
            var elapsedWallClockTime = stopwatch.Elapsed;
            Misc.PrintTime(elapsedWallClockTime);
        }
        if (Renderer.Equals("OnOffRender", StringComparison.OrdinalIgnoreCase))
        {
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new OnOffRenderer(scene.World, Color.Black, Color.Yellow); 
                
            stopwatch.Start();

            tracer.FireAllRays(render.Render);

            stopwatch.Stop();
            var elapsedWallClockTime = stopwatch.Elapsed;
            Misc.PrintTime(elapsedWallClockTime);
        }


        
        console.Output.WriteLine($"File PFM generated!");
        
        var pfmFilePath = "output/" + Path.ChangeExtension(OutputLdrFileName, ".pfm");
        using var pfmStream = new FileStream(pfmFilePath, FileMode.Create, FileAccess.ReadWrite);
        image.WritePfm(pfmStream);
        pfmStream.Seek(0, SeekOrigin.Begin); 

        HdrImage.write_ldr_image(
            pfmStream,
            "output/"+OutputLdrFileName,
            factor: Factor
        );
        console.Output.WriteLine($"Generated LDR: {OutputLdrFileName}");

        return default;
    }
}

[Command("demo", Description = "Generate images with different projections")]
public class DemoCommand : ICommand
{
    [CommandOption("camera", 'c', Description = "Projection type (Perspective/Orthogonal)")]
    public string CameraType { get; init; } = "Perspective";

    [CommandOption("angle", 'a', Description = "Rotation angle Z (degrees)")]
    public float AngleDeg { get; init; } = 0;
    
    [CommandOption("width", 'w', Description = "Image width")]
    public int Width { get; init; } = 1366;

    [CommandOption("height", 'h', Description = "Image height")]
    public int Height { get; init; } = 768;

    [CommandOption("output", 'o', Description = "Output file name (LDR)")]
    public string OutputLdrFileName { get; init; } = "demo.png";
    
    [CommandOption("AntiAliasing", 'A', Description = "Anti-aliasing enabled")]
    public bool AntiAliasing { get; init; } = false;
    
    [CommandOption("NumOfRays", 'R', Description = "Number of rays at each recursion")]
    public int NumOfRays { get; init; } = 1;
    
    [CommandOption("Renderer", 'r', Description = "Renderer type")]
    public string Renderer { get; init; } = "PathTracer";
    

    public Camera? Camera { get; private set; }
    public string OutputPfmFileName => Path.ChangeExtension(OutputLdrFileName, ".pfm");

    public ValueTask ExecuteAsync(IConsole console)
    {
        // Camera type validation
        if(!CameraType.Equals("Perspective", StringComparison.OrdinalIgnoreCase) &&
           !CameraType.Equals("Orthogonal", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"Invalid camera type: {CameraType}");
        }

        // Create transformation
        var rotation = Transformation.RotationZ(AngleDeg);
        var translation = Transformation.Translation(new Vec(-1, 0, 1));
        var transform = rotation * translation;

        // Create camera
        if(CameraType.Equals("Perspective", StringComparison.OrdinalIgnoreCase))
        {
            Camera = new PerspectiveProjection(
                transform: transform,
                aspectRatio: 16.0f/9.0f
            );
        }
        else
        {
            Camera = new OrthogonalProjection(
                transform: transform,
                aspectRatio: 16.0f/9.0f
            );
        }

        // Output information
        console.Output.WriteLine(
            $"Generating PFM file with: Camera = {CameraType}, " +
            $"RotationAlongZ = {AngleDeg.ToString(CultureInfo.InvariantCulture)}, " +
            $"Width = {Width}, Height = {Height}, " +
            $"Output = {OutputLdrFileName}, "+
            $"AntiAliasing = {AntiAliasing}, "+
            $"RendererType = {Renderer}, " +
            $"NumOfRays = {NumOfRays}"
        );

        RunDemoScene();

        return default;
    }
        
    private void RunDemoScene()
    {
        Console.WriteLine("Running demo scene and writing PFM file...");

        var scene = new World();
        //scene 1
        // Create the objects
        
        var sky = new Material
        {
            EmittedRadiance = new UniformPigment(new Color(.05f, 0.05f, 0.05f)),
            Brdf = new DiffusiveBrdf {
                Pigment = new UniformPigment(Color.Black) 
            }
        };
        var ground = new Material
        {
            Brdf = new DiffusiveBrdf {
                Pigment = new CheckeredPigment(new Color(0.3f, 0.5f, 0.1f), new Color(0.1f, 0.2f, 0.5f)) 
            }
        };
        
        var blue = new Material
        {
            Brdf = new DiffusiveBrdf {
                Pigment = new UniformPigment(new Color(0.5f, 0.8f, 1.0f))             
            }        
        };
        
        var red = new Material
        {
            Brdf = new DiffusiveBrdf {
                Pigment = new UniformPigment(new Color(0.7f, 0.2f, 0.2f))             
            }        
        };
        
        var green = new Material
        {
            Brdf = new DiffusiveBrdf {
                Pigment = new UniformPigment(new Color(0.5f, 1.0f, 0.5f))             
            }        
        };
        
        var yellow = new Material
        {
            Brdf = new DiffusiveBrdf {
                Pigment = new UniformPigment(new Color(1.0f, 0.9f, 0f))             
            }        
        };

        var mirrorRed = new Material
        {
            Brdf = new SpecularBrdf {
                Pigment = new UniformPigment(new Color(0.7f, 0.2f, 0.2f))             
            }        
        };
        
       
        // Create the scene
        
        var s1 = new Sphere(
            transformation: Transformation.Translation(new Vec(0.7f, -0.3f, 1.4f)),
            material: red);
        
        var s2 = new Sphere(
            transformation: Transformation.Translation(new Vec(0.7f, -0.3f, 1f)),
            material: blue);
        
        var s3 = new Sphere(
            transformation: Transformation.Scaling(new Vec(1f,1.08f,1f)) * Transformation.Translation(new Vec(0.7f, -0.5f, 1.2f)),
            material: green);
        
        var s4 = new Sphere(
            transformation: Transformation.Scaling(new Vec(1f,1.2f,1f)) * Transformation.Translation(new Vec(0.7f, -0.1f, 1.2f)),
            material: yellow);

        var u1 = new Csg(s1, s2, CsgOperation.Union);
        var u2 = new Csg(u1, s3, CsgOperation.Union);
        var union = new Csg(u2, s4, CsgOperation.Union);
        
        
        scene.AddShape(union);
        
        scene.AddShape(new Box(
            transformation: Transformation.Scaling(new Vec(0.5f,0.5f,0.5f)) 
                            * Transformation.Translation(new Vec(0.1f, 3, 0)) 
                             * Transformation.RotationX(30),
            material:       mirrorRed)
        );
        
        scene.AddShape(new Box(
            transformation: Transformation.Scaling(new Vec(0.5f,0.5f,0.5f)) 
                            * Transformation.Translation(new Vec(2.1f,-6f, 1.5f)) 
                            * Transformation.RotationZ(30) 
                            * Transformation.RotationY(-10),
            material:       mirrorRed)
        );
        
        /*
        scene.AddShape(new Torus(
            transformation: Transformation.Translation(new Vec(2,1,3))
                            * Transformation.RotationZ(60),
            material: blue)
        );*/

        scene.AddShape(new Plane(
            transformation: Transformation.Scaling(new Vec(200,200,200)) * Transformation.Translation(new Vec(0f, 0, 0.4f)),
            material: sky));
        
        scene.AddShape(new Plane(
            material: ground)
        );
        
        var stopwatch = new Stopwatch();
        
        var image = new HdrImage(Width, Height);
        var tracer = new ImageTracer(image, Camera); 
        if (Renderer.Equals("PointLight", StringComparison.OrdinalIgnoreCase))
        {
            scene.AddLight(new PointLight(new Point(-10, 0, 10f), new Color(1, 1, 1)));
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new PointLightRenderer(scene, Color.Black);
            
            stopwatch.Start();

            tracer.FireAllRays(render.Render);

            stopwatch.Stop();
            var elapsedWallClockTime = stopwatch.Elapsed;
            Misc.PrintTime(elapsedWallClockTime);
        }
        
        if (Renderer.Equals("PathTracer", StringComparison.OrdinalIgnoreCase))
        {
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new PathTracer(scene, Color.Black, new Pcg(), NumOfRays, 3, 1);
            
            stopwatch.Start();

            tracer.FireAllRays(render.Render);

            stopwatch.Stop();
            var elapsedWallClockTime = stopwatch.Elapsed;
            Misc.PrintTime(elapsedWallClockTime);
        }
        if (Renderer.Equals("FlatRender", StringComparison.OrdinalIgnoreCase))
        {
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new FlatRenderer(scene, Color.Black);
                
            stopwatch.Start();

            tracer.FireAllRays(render.Render);

            stopwatch.Stop();
            var elapsedWallClockTime = stopwatch.Elapsed;
            Misc.PrintTime(elapsedWallClockTime);
        }
        if (Renderer.Equals("OnOffRender", StringComparison.OrdinalIgnoreCase))
        {
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new OnOffRenderer(scene, Color.Black, Color.Yellow); 
                
            stopwatch.Start();

            tracer.FireAllRays(render.Render);

            stopwatch.Stop();
            var elapsedWallClockTime = stopwatch.Elapsed;
            Misc.PrintTime(elapsedWallClockTime);
        }
        
        using var pfmStream = new MemoryStream();
        image.WritePfm(pfmStream);
        pfmStream.Seek(0, SeekOrigin.Begin);
        HdrImage.write_ldr_image(
            pfmStream,
            "output/"+OutputLdrFileName
        );
        

        Console.WriteLine($"\nGenerated LDR: {OutputLdrFileName}");
    }
}

[Command("pfm2ldr", Description = "Convert PFM to LDR image format (PNG/JPG)")]
public class Pfm2LdrCommand : ICommand
{
    [CommandParameter(0, Name = "input", Description = "Input PFM file path")]
    public string InputPfmFileName { get; init; } = string.Empty;

    [CommandParameter(1, Name = "output", Description = "Output LDR file path")]
    public string OutputLdrFileName { get; init; } = string.Empty;

    [CommandOption("factor", 'f', Description = "Tone mapping scale factor")]
    public float Factor { get; init; } = 0.6f;

    [CommandOption("gamma", 'g', Description = "Gamma correction value")]
    public float Gamma { get; init; } = 1.0f;

    public ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine($"Converting {InputPfmFileName} to {OutputLdrFileName}");
        console.Output.WriteLine($"Parameters: Factor={Factor}, Gamma={Gamma}");

        // Parameter validation
        if (Gamma <= 0)
            throw new CommandException("Gamma value must be greater than zero.");
        if (Factor <= 0)
            throw new CommandException("Factor value must be greater than zero.");

        try
        {
            using var fileStream = File.OpenRead(InputPfmFileName);
            HdrImage.write_ldr_image(
                fileStream,
                OutputLdrFileName,
                Gamma,
                Factor
            );
            console.Output.WriteLine($"Conversion completed successfully, image saved in {OutputLdrFileName}.");
        }
        catch (Exception ex)
        {
            throw new CommandException(
                message: $"Conversion failed: {ex.Message}",
                innerException: ex 
            );
        }

        return default;
    }
}