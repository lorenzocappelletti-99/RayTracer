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
            // Prompt the user to enter the input PFM file name
            Console.Write("Enter the input PFM file name: ");
            InputPfmFileName = Console.ReadLine() ?? "";

            // Prompt the user to enter the output PNG file name
            Console.Write("Enter the output PNG file name: ");
            OutputPngFileName = Console.ReadLine() ?? "";

            // If no arguments are provided, use default values for factor and gamma
            if (args.Length == 0)
            {
                Console.WriteLine("No 'factor' and 'gamma' parameters were specified in the command line.");
                Console.WriteLine($"Default values will be used: Factor = {Factor}, Gamma = {Gamma}.");
            }
            // If two arguments are provided, parse them as factor and gamma
            else if (args.Length == 2)
            {
                if (!float.TryParse(args[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float factor))
                {
                    throw new ArgumentException($"Invalid factor ('{args[0]}'), it must be a floating-point number.");
                }
                Factor = factor;

                if (!float.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float gamma))
                {
                    throw new ArgumentException($"Invalid gamma ('{args[1]}'), it must be a floating-point number.");
                }
                Gamma = gamma;
            }
            // If the number of arguments is incorrect, display usage information
            else
            {
                throw new ArgumentException("Usage: [optional: FACTOR GAMMA]");
            }
        }
    }
}
