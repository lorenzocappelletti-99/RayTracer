// See https://aka.ms/new-console-template for more information

using Trace;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System;
using System.IO; 


namespace Myraytracer
{
    public class Parameters
    {
        public string InputPfmFileName { get; private set; } = "";
        public float Factor { get; private set; } = 0.2f;
        public float Gamma { get; private set; } = 1.0f;
        public string OutputPngFileName { get; private set; } = "";

        public void ParseCommandLine(string[] args)
        {
            if (args.Length != 4)
            {
                throw new ArgumentException("Usage: Myraytracer INPUT_PFM_FILE FACTOR GAMMA OUTPUT_PNG_FILE");
            }

            InputPfmFileName = args[0];

            if (!float.TryParse(args[1], out float factor))
            {
                throw new ArgumentException($"Invalid factor ('{args[1]}'), it must be a floating-point number.");
            }
            Factor = factor;

            if (!float.TryParse(args[2], out float gamma))
            {
                throw new ArgumentException($"Invalid gamma ('{args[2]}'), it must be a floating-point number.");
            }
            Gamma = gamma;

            OutputPngFileName = args[3];
        }
    }
}

namespace Myraytracer
{
    internal abstract class Program
    {
        private static void Main(string[] args)
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
                HdrImage.write_ldr_image(fileStream, parameters.OutputPngFileName, parameters.Gamma, parameters.Factor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
    