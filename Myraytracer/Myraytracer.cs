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
    