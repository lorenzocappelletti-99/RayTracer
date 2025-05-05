/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using Trace;

namespace Myraytracer
{
    internal abstract class Program
    {
        private static void Main(string[] args)
        {
            // THE FOLLOWING MAIN PROVIDES A DEMO SCENE AND IMAGE

            var scene = new World();
            
            var sphere1 = new Sphere(Transformation.Translation(new Vec(+0.5f, +0.5f, +0.5f)), 0.1f);
            var sphere2 = new Sphere(Transformation.Translation(new Vec(-0.5f, +0.5f, +0.5f)), 0.1f);
            var sphere3 = new Sphere(Transformation.Translation(new Vec(-0.5f, -0.5f, +0.5f)), 0.1f);
            var sphere4 = new Sphere(Transformation.Translation(new Vec(+0.5f, -0.5f, +0.5f)), 0.1f);
            var sphere5 = new Sphere(Transformation.Translation(new Vec(+0.5f, +0.5f, -0.5f)), 0.1f);
            var sphere6 = new Sphere(Transformation.Translation(new Vec(-0.5f, +0.5f, -0.5f)), 0.1f);
            var sphere7 = new Sphere(Transformation.Translation(new Vec(-0.5f, -0.5f, -0.5f)), 0.1f);
            var sphere8 = new Sphere(Transformation.Translation(new Vec(+0.5f, -0.5f, -0.5f)), 0.1f);
            var sphere9 = new Sphere(Transformation.Translation(new Vec(+0.0f, +0.0f, -0.5f)), 0.1f);
            var sphere10 = new Sphere(Transformation.Translation(new Vec(+0.0f, +0.5f, +0.0f)), 0.1f);
            
            var observer = new OrthogonalProjection(transform: Transformation.Translation(new Vec(-1.0f, 1.0f, 0)));
            
            scene.AddShape(sphere1);
            scene.AddShape(sphere2);
            scene.AddShape(sphere3);
            scene.AddShape(sphere4);
            scene.AddShape(sphere5);
            scene.AddShape(sphere6);
            scene.AddShape(sphere7);
            scene.AddShape(sphere8);
            scene.AddShape(sphere9);
            scene.AddShape(sphere10);

            var image = new HdrImage(1366, 768);
            var tracer = new ImageTracer(image, observer);
            
            //tracer.FireAllRays(scene);

            //using Stream fileStream = File.OpenRead("first.pfm");
            
            var filePath = "myNewFile.pfm"; // Choose a filename and extension

            using Stream fileStream = File.Create(filePath);
            image.WritePfm(fileStream);


            //HdrImage.write_ldr_image(fileStream, parameters.OutputPngFileName, parameters.Gamma, parameters.Factor);


            // THE FOLLOWING MAIN CONVERTS PFM FILE TO OUTPUT PNG FILE WITH GIVEN a-FACTOR AND GAMMA FACTOR.
            /*
             try
            {
                var parameters = new Parameters();
                parameters.ParseCommandLine(args);

                Console.WriteLine($"PFM File: {parameters.InputPfmFileName}");
                Console.WriteLine($"Factor: {parameters.Factor}");
                Console.WriteLine($"Gamma: {parameters.Gamma}");
                Console.WriteLine($"Output File: {parameters.OutputPngFileName}");


                using Stream fileStream = File.OpenRead(parameters.InputPfmFileName);
                HdrImage.write_ldr_image(fileStream, parameters.OutputPngFileName, parameters.Gamma, parameters.Factor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            */
        }
    }
}
    