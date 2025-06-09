/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using System.ComponentModel.DataAnnotations;
using CliFx;
using CliFx.Exceptions;
using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Globalization;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Trace;

namespace Myraytracer;

[Command("demo", Description = "Generate images with different projections")]
public class DemoCommand : ICommand
{
    [CommandOption("camera", 'c', Description = "Projection type (Perspective/Orthogonal)")]
    [AllowedValues("Perspective", "Orthogonal")]
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
    
    [CommandOption("NumOfRays", 'R', Description = "Number of rays")]
    public int NumOfRays { get; init; } = 3;
    
    [CommandOption("Renderer", 'r', Description = "Renderer type")]
    [AllowedValues("PathTracer", "PointLight", "FlatRender")]
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
        var translation = Transformation.Translation(new Vec(-8, 0, 3));
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
            $"Output={OutputLdrFileName}, "+
            $"AntiAliasing={AntiAliasing}, "+
            $"RendererType={Renderer}"
        );

        RunDemoScene();

        return default;
    }
        
    private void RunDemoScene()
    {
        Console.WriteLine("Running demo scene and writing PFM file...");

        var scene = new World();
        
        // Create the objects
        
        var checkered = new Material
        {
            Brdf = new DiffusiveBrdf {
                Pigment = new CheckeredPigment(new Color(0.5f, 0.75f, 0.5f), new Color(1f, 0.4f, 0.7f)) 
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
                Pigment = new UniformPigment(new Color(0.8f, 0.2f, 0.2f))             
            }        
        };
        
        var green = new Material
        {
            Brdf = new DiffusiveBrdf {
                Pigment = new UniformPigment(new Color(0.5f, 0.75f, 0.5f))             
            }        
        };
        
        var yellow = new Material
        {
            Brdf = new DiffusiveBrdf {
                Pigment = new UniformPigment(new Color(1.0f, 0.9f, 0f))             
            }        
        };

        var mirror = new Material()
        {
            Brdf = new SpecularBrdf() {
                Pigment = new UniformPigment(new Color(0.5f, 0.8f, 1.0f))             
            }        
        };
        
        var sky = new Material
        {
            EmittedRadiance = new UniformPigment(new Color(0.3f, 0.3f, 0.3f)),
            Brdf = new DiffusiveBrdf {
                Pigment = new UniformPigment(Color.Black) 
            }
        };
        
        
        
        // Create the scene


        var box = new Box(
            min: new Vec(-1.5f, -1.5f, -1.5f),
            max: new Vec(1.5f, 1.5f, 1.5f),
            transformation: Transformation.Translation(new Vec(0,0,2f)),
            material: red
        );        
        
        var sphere = new Sphere(material: blue,
            transformation: Transformation.Translation(new Vec(2f,0,1.5f)),
            radius: 0.8f);
        
        var dif = new Csg(box, sphere, CsgOperation.Difference);
        
        scene.AddShape(box);
        
        
        //sky
        scene.AddShape(new Plane(material:sky, transformation: Transformation.Translation(new Vec(0, 0, 100)) ));
        
        //ground
        scene.AddShape(new Plane(material: checkered));
        
        /*
        
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
            transformation: Transformation.Translation(new Vec(2.5f, -0.1f, 1.2f)),
            material: red);
        
        
        var s = new Sphere(
            transformation: Transformation.Translation(new Vec(1.5f, 3f, 0f)),
            material: new Material
            {
                EmittedRadiance = new UniformPigment(new Color(0.3f, 0.5f, 0.1f)),
                Brdf = new DiffusiveBrdf {
                    Pigment = new UniformPigment(Color.Black) 
                }
            }
            );
            
            */
        

        //var difference = new Csg(sphere1, sphere2, CsgOperation.Difference);
        //var intersection = new Csg(sphere1, sphere2, CsgOperation.Intersection);

       // var u1 = new Csg(s1, s2, CsgOperation.Union);
//var u2 = new Csg(u1, s3, CsgOperation.Union);
      //  var union = new Csg(u2, s4, CsgOperation.Union);
        
        //scene.AddShape(union);
        //scene.AddShape(s5);
        
        
        //scene.AddShape(new Plane(
        //    transformation: Transformation.Translation(new Vec(0f, 0, 4f)),
         //   material: ground));
        
        //scene.AddShape(new Plane(
        //    material: ground)
       // );
        
        //scene.AddShape(union);
        
        
        
        /*
        //////////////////////////////////
        ////    ROOM WITH SPHERE   //////
        ////////////////////////////////
        
        var sky = new Material
           {
               EmittedRadiance = new UniformPigment(new Color(0.2f, 0.2f, 0.2f)),
               Brdf = new DiffusiveBrdf {
                   Pigment = new UniformPigment(Color.Black) 
               }
           };
           
           var checkered = new Material
           {
               Brdf = new DiffusiveBrdf {
                   Pigment = new CheckeredPigment(new Color(0.5f, 0.75f, 0.5f), new Color(1f, 0.4f, 0.7f)) 
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
                   Pigment = new UniformPigment(new Color(0.8f, 0.2f, 0.2f))             
               }        
           };
        
        var s4 = new Sphere(
           transformation: Transformation.Translation(new Vec(2.5f, -0.1f, 1.2f)),
           material: red);
        
        //parete dx
        scene.AddShape(new Plane(
            transformation: Transformation.Translation(new Vec(0f, 4f, 0f)) * Transformation.RotationX(90f),
            material: blue));
        
        //parete sx
        scene.AddShape(new Plane(
            transformation: Transformation.Translation(new Vec(0f, -4f, 0f)) * Transformation.RotationX(90f),
            material: blue));
            
        //soffitto
        scene.AddShape(new Plane(
            transformation: Transformation.Translation(new Vec(0f, 0f, 8f)),
            material: sky));
        
        //parete in fondo
        scene.AddShape(new Plane(
            transformation: Transformation.Translation(new Vec(7f, 0f, 0f)) * Transformation.RotationY(90f),
            material: blue));
        
        //parete alle spalle
        scene.AddShape(new Plane(
            transformation: Transformation.Translation(new Vec(-2f, 0f, 0f)) * Transformation.RotationY(90f),
            material: blue));
        
        //pavimento
        scene.AddShape(new Plane(material: checkered));
        
        //sfera
        scene.AddShape(s4);
        
        */
        
        
        
        if (Renderer.Equals("PointLight", StringComparison.OrdinalIgnoreCase))
        {
            scene.AddLight(new PointLight(new Point(-10, 0, 10f), new Color(1, 1, 1)));
            var image = new HdrImage(Width, Height);
            var tracer = new ImageTracer(image, Camera);
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new PointLightRenderer(scene, Color.Black);
            tracer.FireAllRays(render.Render);
            using var pfmStream = new MemoryStream();
            image.WritePfm(pfmStream);
            pfmStream.Seek(0, SeekOrigin.Begin);
            HdrImage.write_ldr_image(
                pfmStream,
                OutputLdrFileName
            );
        }
        
        if (Renderer.Equals("PathTracer", StringComparison.OrdinalIgnoreCase))
        {
            var image = new HdrImage(Width, Height);
            var tracer = new ImageTracer(image, Camera);
            if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new PathTracer(scene, Color.Black, new Pcg(), NumOfRays, 3, 1);
            tracer.FireAllRays(render.Render);
            using var pfmStream = new MemoryStream();
            image.WritePfm(pfmStream);
            pfmStream.Seek(0, SeekOrigin.Begin);
            HdrImage.write_ldr_image(
                pfmStream,
                OutputLdrFileName
            );
        }
        
        if (Renderer.Equals("FlatRender", StringComparison.OrdinalIgnoreCase))
        {
            var image = new HdrImage(Width, Height);
            var tracer = new ImageTracer(image, Camera);
            //if (AntiAliasing) tracer.SamplesPerSide = 4;
            var render = new FlatRenderer(scene, Color.Black);
            tracer.FireAllRays(render.Render);
            using var pfmStream = new MemoryStream();
            image.WritePfm(pfmStream);
            pfmStream.Seek(0, SeekOrigin.Begin);
            HdrImage.write_ldr_image(
                pfmStream,
                OutputLdrFileName
            );
        }
        

        
        

        

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