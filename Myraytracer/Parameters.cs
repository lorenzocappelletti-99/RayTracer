/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using System.Globalization;
using Trace;

namespace Myraytracer
{
    public class ParametersPfm2Png
    {
        public string InputPfmFileName { get; private set; } = "";
        public float Factor { get; private set; } = 0.6f;
        public float Gamma { get; private set; } = 1.0f;
        public string OutputPngFileName { get; private set; } = "";

        public void ParseCommandLine(string[] args)
        {
            // args[0] è "pfm2png", quindi servono almeno args[1] e args[2]
            if (args.Length < 3)
            {
                throw new ArgumentException("Usage: pfm2png <input.pfm> <output.png> [factor] [gamma]");
            }

            InputPfmFileName = args[1];
            OutputPngFileName = args[2];

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
        public string OutputPngFileName { get; private set; } = "Demo.png";
        
        public void ParseCommandLine(string[] args)
        {
            if (args.Length == 1)
            {
                Console.WriteLine("Generating PFM file with: Camera=Perspective, AngleDeg=0, ImageWidth=1366, ImageHeight=768");
            }
            
            if (args.Length == 4 && !float.TryParse(args[3], NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            {
                OutputPngFileName = args[3];
                
            }

            
            
            // args[0] è "demo", quindi potrebbero servire args[1], args[2], args[3], args[4]
            if (args.Length == 2)
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
            }
            
            

            if (args.Length == 3)
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
            }
            

            if (args.Length == 4)
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
            }

            if (args.Length == 5)
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
                if (int.TryParse(args[3], out var value3) && int.TryParse(args[4], out var value4))
                {
                    Width = value3;
                    Height = value4;
                }
                else
                {
                    throw new ArgumentException(
                        $"Invalid Width ('{args[3]}') or Height ('{args[4]}'), it must be a floating-point number.");
                }
            }

        }
    }
}
