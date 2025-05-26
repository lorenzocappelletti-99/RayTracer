/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using CliFx;
using CliFx.Exceptions;
using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Globalization;
using Trace;

namespace Myraytracer;

[Command("demo", Description = "Generate images with different projections")]
public class DemoCommand : ICommand
{
    [CommandParameter(0, Name = "camera", Description = "Projection type (Perspective/Orthogonal)")]
    public string CameraType { get; init; } = "Perspective";

    [CommandOption("angle", 'a', Description = "Rotation angle Z (degrees)")]
    public float AngleDeg { get; init; } = 0;
    
    [CommandOption("width", 'w', Description = "Image width")]
    public int Width { get; init; } = 1366;

    [CommandOption("height", 'h', Description = "Image height")]
    public int Height { get; init; } = 768;

    [CommandOption("output", 'o', Description = "Output file name (LDR)")]
    public string OutputLdrFileName { get; init; } = "demo.png";

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
            $"Generating PFM file with: Camera={CameraType}, " +
            $"AngleDeg={AngleDeg.ToString(CultureInfo.InvariantCulture)}, " +
            $"Width={Width}, Height={Height}, " +
            $"Output={OutputLdrFileName}"
        );

        RunDemoScene();

        return default;
    }
        
    private void RunDemoScene()
    {
        Console.WriteLine("Running demo scene and writing PFM file...");

        var scene = new World();
        
        // Create the objects
        
        var sky = new Material
        {
            EmittedRadiance = new UniformPigment(new Color(1.0f, 0.9f, 0.5f)),
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
        var sphere = new Material
        {
            Brdf = new DiffusiveBrdf {
                Pigment = new UniformPigment(new Color(0.3f, 0.4f, 0.8f))             
            }        
        };

        var mirror = new Material()
        {
            Brdf = new SpecularBrdf() {
                Pigment = new UniformPigment(new Color(0.6f, 0.2f, 0.3f))             
            }        
        };
        
        // Create the scene

        scene.AddShape(new Sphere(
            transformation: Transformation.Translation(new Vec(0f, 0, 1f)),
            material: sphere));
        
        scene.AddShape(new Sphere(
            transformation: Transformation.Translation(new Vec(1f, 2.5f, 0f)),
            material: mirror));
        
        scene.AddShape(new Plane(
            material: ground
            )
        );
        
        scene.AddShape(new Plane(
            transformation: Transformation.Scaling(new Vec(200,200,200)) * Transformation.Translation(new Vec(0f, 0, 0.4f)),
            material: sky));
        

        var image = new HdrImage(Width, Height);
        var render = new PathTracer(world: scene, pcg: new Pcg(), numOfRays: 10, maxDepth: 3, russianRouletteLimit: 1);
        var tracer = new ImageTracer(image, Camera, 10, new Pcg());
        tracer.FireAllRays(render.Render);
        
        
        using var pfmStream = new MemoryStream();
        image.WritePfm(pfmStream);
        pfmStream.Seek(0, SeekOrigin.Begin);

        HdrImage.write_ldr_image(
            pfmStream,
            OutputLdrFileName
        );

        Console.WriteLine($"Generated LDR: {OutputLdrFileName}");
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