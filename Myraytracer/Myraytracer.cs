/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using Trace;

namespace Myraytracer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0 || args[0].Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  dotnet run demo                -> run demo");
                Console.WriteLine("  dotnet run pfm2png [options]   -> convert PFM to PNG");
                Console.WriteLine("  dotnet run help                -> show this message");
                return;
            }
            
            // Se non ci sono argomenti, o il primo è "demo", eseguo la demo scene→PFM
            if (args[0].Equals("demo", StringComparison.OrdinalIgnoreCase))
            {
                RunDemoScene();
                return;
            }

            // Altrimenti provo a fare la conversione PFM→PNG con la classe Parameters
            if (args[0].Equals("pfm2png", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var parameters = new Parameters();
                    parameters.ParseCommandLine(args);

                    Console.WriteLine($"PFM File: {parameters.InputPfmFileName}");
                    Console.WriteLine($"Factor: {parameters.Factor}");
                    Console.WriteLine($"Gamma: {parameters.Gamma}");
                    Console.WriteLine($"Output File: {parameters.OutputPngFileName}");

                    using Stream fileStream = File.OpenRead(parameters.InputPfmFileName);
                    HdrImage.write_ldr_image(
                        fileStream,
                        parameters.OutputPngFileName,
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
        private static void RunDemoScene()
        {
            Console.WriteLine("Running demo scene and writing PFM file...");

            var scene = new World();
            
            scene.AddShape(new Sphere(0.1f, Transformation.Translation(new Vec(+0.5f, +0.5f, +0.5f))));
            scene.AddShape(new Sphere(0.1f, Transformation.Translation(new Vec(-0.5f, +0.5f, +0.5f))));
            scene.AddShape(new Sphere(0.1f, Transformation.Translation(new Vec(-0.5f, -0.5f, +0.5f))));
            scene.AddShape(new Sphere(0.1f, Transformation.Translation(new Vec(+0.5f, -0.5f, +0.5f))));
            scene.AddShape(new Sphere(0.1f, Transformation.Translation(new Vec(+0.5f, +0.5f, -0.5f))));
            scene.AddShape(new Sphere(0.1f, Transformation.Translation(new Vec(-0.5f, +0.5f, -0.5f))));
            scene.AddShape(new Sphere(0.1f, Transformation.Translation(new Vec(-0.5f, -0.5f, -0.5f))));
            scene.AddShape(new Sphere(0.1f, Transformation.Translation(new Vec(+0.5f, -0.5f, -0.5f))));
            scene.AddShape(new Sphere(0.1f, Transformation.Translation(new Vec(+0.0f, +0.0f, -0.5f))));
            scene.AddShape(new Sphere(0.1f, Transformation.Translation(new Vec(+0.0f, +0.5f, +0.0f))));


            // Observer e tracer
            var observer = new OrthogonalProjection(
                    transform: Transformation.Translation(new Vec(-1f, 0f, 0f)) * Transformation.RotationY(90)
                );
            
            var image  = new HdrImage(1366, 768);
            var tracer = new ImageTracer(image, observer);
            
            tracer.FireAllRays(scene);

            // Scrivo il file PFM
            var filePath = "myNewFile.pfm";
            using var fs = File.Create(filePath);
            image.WritePfm(fs);

            Console.WriteLine($"Demo scene written to {filePath}");
        }
    }
}
