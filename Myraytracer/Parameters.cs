/*===========================================================
 |                     Raytracer Project                    
 |             Released under EUPL-1.2 License               
 |                       See LICENSE                        
 ===========================================================*/

using System.Globalization;

namespace Myraytracer
{
    public class Parameters
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
}
