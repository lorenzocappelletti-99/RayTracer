/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using System.Globalization;
using Trace;

namespace Myraytracer
{
    public class ParametersPfm2Jpg
    {
        public string InputPfmFileName { get; private set; } = "";
        public float Factor { get; private set; } = 0.6f;
        public float Gamma { get; private set; } = 1.0f;
        public string OutputJpgFileName { get; private set; } = "";

        public void ParseCommandLine(string[] args)
        {
            // args[0] is "pfm2png", need at least args[1] e args[2]
            if (args.Length < 3)
            {
                throw new ArgumentException("Usage: pfm2jpg <input.pfm> <output.jpg> [factor] [gamma]");
            }

            InputPfmFileName = args[1];
            OutputJpgFileName = args[2];

            if (args.Length > 3)
            {
                if (!float.TryParse(args[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float factor))
                {
                    throw new ArgumentException($"Invalid factor ('{args[3]}'), it must be a floating-point number.");
                }

                Factor = factor;
            }

            if (args.Length > 4)
            {
                if (!float.TryParse(args[4], NumberStyles.Float, CultureInfo.InvariantCulture, out float gamma))
                {
                    throw new ArgumentException($"Invalid gamma ('{args[4]}'), it must be a floating-point number.");
                }

                Gamma = gamma;
            }
        }

    }


    public class ParametersDemo
    {
        public int Width { get; private set; } = 1366;
        public int Height { get; private set; } = 768;
        public float AngleDeg { get; private set; }
        public Camera Camera { get; private set; } = 
            new PerspectiveProjection(16/9f, transform: Transformation.Translation(new Vec(-1, 0,0)));
        public string OutputPngFileName { get; private set; } = "Demo.jpg";
        public string OutputPfmFileName { get; private set; } = "Demo.pfm";

        
        public void ParseCommandLine(string[] args)
        {
            switch (args.Length)
            {
                case 2 when args[1].Equals("help", StringComparison.OrdinalIgnoreCase):
                    throw new ArgumentException("\nUsage:" +
                                                "\ndotnet run demo [Camera] [AngleDeg] [Width] [Height]" +
                                                "\ndotnet run demo [Camera] [AngleDeg] [output.jpg]  " +
                                                "\ndotnet run demo help");
                case 1:
                    Console.WriteLine("Generating PFM file with: Camera = "+"Perspective"+
                                      ", AngleDeg = "+AngleDeg+", ImageWidth = "+Width+", ImageHeight = "+Height);
                    break;
            }

            switch (args.Length)
            {
                case 1:
                {
                    AngleDeg = 0;
                    break;
                }
                case 2 when args[1] == "PerspectiveProjection" || args[1] == "OrthogonalProjection":
                {
                    var input = args[1].Trim();
                    if (input.Equals("PerspectiveProjection", StringComparison.OrdinalIgnoreCase))
                    {
                        Camera = new PerspectiveProjection();
                    }
                    else if (input.Equals("OrthogonalProjection", StringComparison.OrdinalIgnoreCase))
                    {
                        Camera = new OrthogonalProjection();
                    }
                    else
                    {
                        throw new ArgumentException(
                            $"Invalid Camera ('{args[1]}'), it must be a camera: choose between Perspective Projection and Orthogonal Projection."
                        );
                    }
                    Console.WriteLine("Generating PFM file with: Camera"+args[1]+
                                      ", AngleDeg"+args[2]+", ImageWidth="+Width+", ImageHeight="+Height);
                    break;
                }
                case 3:
                {
                    if (float.TryParse(args[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                    {
                        AngleDeg = value;
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid AngleDeg ('{args[2]}'), it must be a floating-point number.");
                    }
                
                    var input = args[1].Trim();
                    if (input.Equals("PerspectiveProjection", StringComparison.OrdinalIgnoreCase))
                    {
                        Camera = new PerspectiveProjection(transform: Transformation.Translation(new Vec(-1, 0,0))*Transformation.RotationX(value));
                    }
                    else if (input.Equals("OrthogonalProjection", StringComparison.OrdinalIgnoreCase))
                    {
                        Camera = new OrthogonalProjection(transform: Transformation.Translation(new Vec(-1, 0,0))*Transformation.RotationX(value));
                    }
                    else
                    {
                        throw new ArgumentException(
                            $"Invalid Camera ('{args[1]}'), it must be a camera: choose between Perspective Projection and Orthogonal Projection."
                        );
                    }
                    Console.WriteLine("Generating PFM file with: Camera"+args[1]+
                                      ", AngleDeg"+args[2]+", ImageWidth="+Width+", ImageHeight="+Height);
                    break;
                }
                case 4 when float.TryParse(args[3], NumberStyles.Float, CultureInfo.InvariantCulture, out _):
                {
                    var input = args[1].Trim();
                    if (input.Equals("PerspectiveProjection", StringComparison.OrdinalIgnoreCase))
                    {
                        Camera = new PerspectiveProjection();
                    }
                    else if (input.Equals("OrthogonalProjection", StringComparison.OrdinalIgnoreCase))
                    {
                        Camera = new OrthogonalProjection();
                    }
                    else
                    {
                        throw new ArgumentException(
                            $"Invalid Camera ('{args[1]}'), it must be a camera: choose between Perspective Projection and Orthogonal Projection."
                        );
                    }
                    if (float.TryParse(args[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                    {
                        AngleDeg = value;
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid AngleDeg ('{args[2]}'), it must be a floating-point number.");
                    }
                    if(int.TryParse(args[3], out var width)) {Width = width;}
                    Console.WriteLine("Generating PFM file with: Camera"+args[1]+
                                      ", AngleDeg"+args[2]+", ImageWidth="+Width+", ImageHeight="+Height);
                    break;
                }
            }


            switch (args.Length)
            {
                case 4 when !float.TryParse(args[3], NumberStyles.Float, CultureInfo.InvariantCulture, out _):
                {
                    OutputPngFileName = args[3];
                    if (float.TryParse(args[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                    {
                        AngleDeg = value;
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid AngleDeg ('{args[2]}'), it must be a floating-point number.");
                    }
                
                    var input = args[1];
                    if (input.Equals("PerspectiveProjection", StringComparison.OrdinalIgnoreCase))
                    {
                        Camera = new PerspectiveProjection(transform: Transformation.Translation(new Vec(-1, 0,0))*Transformation.RotationX(AngleDeg), aspectRatio: 16/9f );
                    }
                    else if (input.Equals("OrthogonalProjection", StringComparison.OrdinalIgnoreCase))
                    {
                        Camera = new OrthogonalProjection(transform: Transformation.Translation(new Vec(-1, 0,0))*Transformation.RotationX(AngleDeg), aspectRatio: 16/9f);
                    }
                    else
                    {
                        throw new ArgumentException(
                            $"Invalid Camera ('{args[1]}'), it must be a camera: choose between PerspectiveProjection and OrthogonalProjection."
                        );
                    }
                    Console.WriteLine("Generating PFM file with: Camera"+args[1]+
                                      ", AngleDeg"+args[2]+", ImageWidth="+Width+", ImageHeight="+Height);
                    break;
                }
                case 6:
                {
                    var input = args[1].Trim();
                    if (input.Equals("PerspectiveProjection", StringComparison.OrdinalIgnoreCase))
                    {
                        Camera = new PerspectiveProjection();
                    }
                    else if (input.Equals("OrthogonalProjection", StringComparison.OrdinalIgnoreCase))
                    {
                        Camera = new OrthogonalProjection();
                    }
                    else
                    {
                        throw new ArgumentException(
                            $"Invalid Camera ('{args[1]}'), it must be a camera: choose between PerspectiveProjection and OrthogonalProjection."
                        );
                    }

                    if (float.TryParse(args[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var angle))
                    {
                        AngleDeg = angle;
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid AngleDeg ('{args[2]}'), it must be a floating-point number.");
                    }

                    if (int.TryParse(args[3], out var width) && int.TryParse(args[4], out var height))
                    {
                        Width = width;
                        Height = height;
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid Width or Height ('{args[3]}', '{args[4]}'), must be integers.");
                    }

                    OutputPngFileName = args[5];
                    // generiamo il nome del PFM sostituendo l’estensione
                    OutputPfmFileName = Path.ChangeExtension(OutputPngFileName, ".pfm");

                    Console.WriteLine("Generating PFM file with: Camera=" + args[1] +
                                      ", AngleDeg=" + args[2] +
                                      ", Width=" + Width +
                                      ", Height=" + Height +
                                      ", Output=" + OutputPngFileName);
                    break;
                }

            }
        }
    }
}
