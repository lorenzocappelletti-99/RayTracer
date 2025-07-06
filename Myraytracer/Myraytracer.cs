/*===========================================================
 |                     Raytracer Project
 |             Released under EUPL-1.2 License
 |                       See LICENSE
 ===========================================================*/

using CliFx;

namespace Myraytracer;

internal static class Program
{
    public static async Task<int> Main()
    {
        return await new CliApplicationBuilder()
            .AddCommand<RenderCommand>()
            .AddCommand<DemoCommand>()
            .AddCommand<Pfm2LdrCommand>()
            .Build()
            .RunAsync();
    }
    
    
}