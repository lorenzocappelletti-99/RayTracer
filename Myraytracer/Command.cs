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
        var translation = Transformation.Translation(new Vec(-1, 0, 0));
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
        
        var yellowCheck = new Material
        {
            EmittedRadiance = new CheckeredPigment(Color.Yellow, Color.Black)  
        };
        var red = new Material
        {
            EmittedRadiance = new UniformPigment(Color.Red)  
        };
        var checkeredMaterial = new Material
        {
            EmittedRadiance = new CheckeredPigment(Color.Green, Color.Purple)  
        };

        var bluecheck = new Material()
        {
            EmittedRadiance = new CheckeredPigment(Color.Blue,Color.Black)
        };

        scene.AddShape(new Sphere(
            radius: 0.1f,
            transformation: Transformation.Translation(new Vec(-1.5f, 0, 0f)),
            material: checkeredMaterial));
        
        scene.AddShape(new Sphere(
            radius: 0.1f,
            transformation: Transformation.Translation(new Vec(-0.9f, 1f, 0.5f)),
            material: yellowCheck));
        
        scene.AddShape(new Plane(
            transformation: Transformation.Translation(new Vec(1000f, 0, 0f)) * Transformation.RotationY(90),
            material: red));
        
        scene.AddShape(new Plane(
            transformation: Transformation.Translation(new Vec(0f, 0, -0.2f)),
            material: bluecheck));

        var image = new HdrImage(Width, Height);
        var tracer = new ImageTracer(image, Camera);
        var render = new FlatRenderer(scene);
        tracer.FireAllRays(scene, render.Render);

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