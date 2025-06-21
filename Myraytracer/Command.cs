/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using CliFx;
using CliFx.Exceptions;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Trace;

namespace Myraytracer;

[Command("render", Description = "Parse a scene file and render it.")]
public class RenderCommand : ICommand
{
    [CommandParameter(0, Name = "scene-file", Description = "Path of the input scene-file to render.")]
    public string SceneFile { get; init; } = "";

    [CommandOption("width", 'w', Description = "Image width")]
    public int Width { get; init; } = 800;

    [CommandOption("height", 'h', Description = "Image height")]
    public int Height { get; init; } = 600;

    [CommandOption("output", 'o', Description = "Output file name (LDR)")]
    public string OutputLdrFileName { get; init; } = "output.png";

    [CommandOption("anti-aliasing", 'A', Description = "Enable anti-aliasing")]
    public bool AntiAliasing { get; init; } = false;

    [CommandOption("renderer", 'r', Description = "Renderer type: PointLight|PathTracer")]
    public string Renderer { get; init; } = "PathTracer";

    public async ValueTask ExecuteAsync(IConsole console)
    {
<<<<<<< Updated upstream
        // 1) Carico e parsifico il file di scena
        if (!File.Exists(SceneFile))
            throw new CommandException($"File not found: {SceneFile}");
=======
        if (InputFile == null) return default;
        StreamReader reader;
        InputStream inputStream;
        try
        {
            reader = new StreamReader(InputFile);
            inputStream = new InputStream(reader, InputFile);   
        }
        catch
        {
            InputFile = "input/" + InputFile;
            reader = new StreamReader(InputFile);
            inputStream = new InputStream(reader, InputFile);   
        }
        
        //PARSING
        Scene? scene = null;
        try
        {
            console.WriteLine("Parsing scene...");
            scene = Scene.ParseScene(inputStream);
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
>>>>>>> Stashed changes

        Scene scene;
        using (var reader = new StreamReader(SceneFile))
        {
            var input = new InputStream(reader, SceneFile);
            scene = Scene.ParseScene(input);
        }

        // 2) Preparo l'immagine e il tracer
        var image = new HdrImage(Width, Height);
        var tracer = new ImageTracer(image, scene.Camera!);

        if (AntiAliasing)
            tracer.SamplesPerSide = 4;

        // 3) Aggiungo luci o path‐tracer a seconda dell’opzione
        if (Renderer.Equals("PointLight", StringComparison.OrdinalIgnoreCase))
        {
            var pointLightRndr = new PointLightRenderer(scene.World, Color.Black);
            tracer.FireAllRays(pointLightRndr.Render);
        }
        else if (Renderer.Equals("PathTracer", StringComparison.OrdinalIgnoreCase))
        {
<<<<<<< Updated upstream
            var pathTracer = new PathTracer(
                scene.World,
                Color.Black,
                new Pcg(), // seed PRNG
                3, 3, 1 // depth, russian roulette, …
=======
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new PathTracer(scene.World, Color.Black, new Pcg(), NumOfRays, MaxDepth, RussianRoulette);
            
            stopwatch.Start();

            tracer.FireAllRays(render.Render);

            stopwatch.Stop();
            var elapsedWallClockTime = stopwatch.Elapsed;
            Misc.PrintTime(elapsedWallClockTime);
        }
        else if (Renderer.Equals("FlatRender", StringComparison.OrdinalIgnoreCase))
        {
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new FlatRenderer(scene.World, Color.Black);
            
            stopwatch.Start();

            tracer.FireAllRays(render.Render);

            stopwatch.Stop();
            var elapsedWallClockTime = stopwatch.Elapsed;
            Misc.PrintTime(elapsedWallClockTime);
        }
        else if (Renderer.Equals("OnOffRender", StringComparison.OrdinalIgnoreCase))
        {
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new OnOffRenderer(scene.World, Color.White);
            
            stopwatch.Start();

            tracer.FireAllRays(render.Render);

            stopwatch.Stop();
            var elapsedWallClockTime = stopwatch.Elapsed;
            Misc.PrintTime(elapsedWallClockTime);
        }

        console.Output.WriteLine($"File PFM generated!");
        
        string pfmFilePath = "output/" + Path.ChangeExtension(OutputLdrFileName, ".pfm");

        using var pfmStream = new FileStream(pfmFilePath, FileMode.Create, FileAccess.ReadWrite);
        image.WritePfm(pfmStream);
        pfmStream.Seek(0, SeekOrigin.Begin); // Optional if HdrImage.write_ldr_image reads from beginning

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
>>>>>>> Stashed changes
            );
            tracer.FireAllRays(pathTracer.Render);
        }
        else
        {
            throw new CommandException($"Unknown renderer: {Renderer}");
        }

        // 4) Scrivo su file
        using var pfmStream = new MemoryStream();
        image.WritePfm(pfmStream);
        pfmStream.Seek(0, SeekOrigin.Begin);
        HdrImage.write_ldr_image(pfmStream, OutputLdrFileName);

        console.Output.WriteLine($"Generated LDR image: {OutputLdrFileName}");
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