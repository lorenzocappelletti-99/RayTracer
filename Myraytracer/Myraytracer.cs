/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using Trace;

namespace Myraytracer;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length == 0 || args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run demo    [options]   -> run demo");
            Console.WriteLine("  dotnet run pfm2ldr [options]   -> convert PFM to ldr (png or jpg)");
            Console.WriteLine("  dotnet run help                -> show this message");
            return;
        }

        // Se non ci sono argomenti, o il primo è "demo", eseguo la demo scene→PFM
        if (args[0].Equals("demo", StringComparison.OrdinalIgnoreCase))
        {
            var parameters = new ParametersDemo();
            parameters.ParseCommandLine(args);
            RunDemoScene(parameters);
            return;
        }

        // Altrimenti provo a fare la conversione PFM→Ldr con la classe Parameters
        if (args[0].Equals("pfm2ldr", StringComparison.OrdinalIgnoreCase))
        {
            try
            {

                var parameters = new ParametersPfm2Ldr();
                parameters.ParseCommandLine(args);

                Console.WriteLine($"PFM File: {parameters.InputPfmFileName}");
                Console.WriteLine($"Factor: {parameters.Factor}");
                Console.WriteLine($"Gamma: {parameters.Gamma}");
                Console.WriteLine($"Output File: {parameters.OutputLdrFileName}");

                using Stream fileStream = File.OpenRead(parameters.InputPfmFileName);
                HdrImage.write_ldr_image(
                    fileStream,
                    parameters.OutputLdrFileName,
                    parameters.Gamma,
                    parameters.Factor
                );

                Console.WriteLine("Conversion completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during conversion: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Builds a demo scene of spheres and writes it to "myNewFile.pfm".
    /// </summary>
    private static void RunDemoScene(ParametersDemo parameters)
    {

        Console.WriteLine("Running demo scene and writing PFM file...");

        // Costruzione della scena
        var scene = new World();
            
        /*
        var yellowCheckered = new Material
        {
            Pigment = new CheckeredPigment(Color.Yellow, Color.Black, 5)  
        };
        var greenCheckered = new Material
        {
            Pigment = new CheckeredPigment(Color.Green, Color.Purple)  
        };
        */
        
        var yellowMaterial = new Material
        {
            Pigment = new UniformPigment(Color.Yellow)  
        };
        var checkeredMaterial = new Material
        {
            Pigment = new CheckeredPigment(Color.Green, Color.Purple)  
        };
            
        
        
        // Spheres at the corners (yellow)
        var corners = new[]
        {
            new Vec(+0.5f, +0.5f, +0.5f),
            new Vec(-0.5f, +0.5f, +0.5f),
            new Vec(-0.5f, -0.5f, +0.5f),
            new Vec(+0.5f, -0.5f, +0.5f),
            new Vec(+0.5f, +0.5f, -0.5f),
            new Vec(-0.5f, +0.5f, -0.5f),
            new Vec(-0.5f, -0.5f, -0.5f),
            new Vec(+0.5f, -0.5f, -0.5f),
        };
        foreach (var v in corners)
        {
            scene.AddShape(new Sphere(
                radius: 0.1f,
                transformation: Transformation.Translation(v),
                material: yellowMaterial));
        }

        // Two face-centers (checkered)
        scene.AddShape(new Sphere(
            radius: 0.1f,
            transformation: Transformation.Translation(new Vec(0, 0, -0.5f)),
            material: checkeredMaterial));
        scene.AddShape(new Sphere(
            radius: 0.1f,
            transformation: Transformation.Translation(new Vec(0, 0.5f,  0)),
            material: checkeredMaterial));
        
        
        /*
        scene.AddShape(new Sphere(
            radius:0.1f,
            transformation: Transformation.Translation(new Vec(0f,0.5f,0f)),
            material: yellowCheckered));
        
        //scene.AddShape(new Plane(
        //    material: greenCheckered));
        
        */

        var image  = new HdrImage(parameters.Width, parameters.Height);
        var tracer = new ImageTracer(image, parameters.Camera);
        tracer.FireAllRaysFlat(scene);

        // --- OUTPUT PATHS DA PARAMETRI ---
        var ldrPath = parameters.OutputLdrFileName;

        // 1) Crea le directory di destinazione se necessario
        var ldrDir = Path.GetDirectoryName(ldrPath);
        if (!string.IsNullOrEmpty(ldrDir)) Directory.CreateDirectory(ldrDir);

        using var pfmStream = new MemoryStream();
        image.WritePfm(pfmStream);
        pfmStream.Seek(0, SeekOrigin.Begin);

        HdrImage.write_ldr_image(
            pfmStream,
            ldrPath
        );

        Console.WriteLine($"Generated ldr: {ldrPath}");
    }

}