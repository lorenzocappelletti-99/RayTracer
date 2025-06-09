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
        // 1) Carico e parsifico il file di scena
        if (!File.Exists(SceneFile))
            throw new CommandException($"File not found: {SceneFile}");

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
            var pathTracer = new PathTracer(
                scene.World,
                Color.Black,
                new Pcg(), // seed PRNG
                3, 3, 1 // depth, russian roulette, …
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