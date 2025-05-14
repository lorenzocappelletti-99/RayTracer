/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using System.Reflection.Metadata;
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
                Console.WriteLine("  dotnet run demo    [options]   -> run demo");
                Console.WriteLine("  dotnet run pfm2png [options]   -> convert PFM to JPG");
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

            // Altrimenti provo a fare la conversione PFM→JPG con la classe Parameters
            if (args[0].Equals("pfm2jpg", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    
                    var parameters = new ParametersPfm2Jpg();
                    parameters.ParseCommandLine(args);

                    Console.WriteLine($"PFM File: {parameters.InputPfmFileName}");
                    Console.WriteLine($"Factor: {parameters.Factor}");
                    Console.WriteLine($"Gamma: {parameters.Gamma}");
                    Console.WriteLine($"Output File: {parameters.OutputJpgFileName}");

                    using Stream fileStream = File.OpenRead(parameters.InputPfmFileName);
                    HdrImage.write_ldr_image(
                        fileStream,
                        parameters.OutputJpgFileName,
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
            var observer = new PerspectiveProjection(
                transform: Transformation.Translation(new Vec(-1, 0,0))*Transformation.RotationZ(parameters.AngleDeg));
            
            var image  = new HdrImage(parameters.Width, parameters.Height);
            var tracer = new ImageTracer(image, observer);
            
            tracer.FireAllRays(scene);

            const string filePath = "Demo.pfm";

            using (var outputStream = File.Create(filePath))
            {
                image.WritePfm(outputStream);
            }

            Console.WriteLine($"Demo scene written to {filePath}");

            using var inputStream = File.OpenRead(filePath);
            HdrImage.write_ldr_image(
                inputStream,
                parameters.OutputPngFileName,
                1f,
                1f
            );

        }
    }
}
