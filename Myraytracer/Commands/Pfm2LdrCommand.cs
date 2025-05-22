using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace Myraytracer.Commands;

[Command("pfm2ldr", Description = "Converts a PFM file to an LDR image.")]
public class Pfm2LdrCommand : ICommand
{
    [CommandParameter(0, Name = "input", Description = "Input PFM file path")]
    private string InputPfmFileName { get; set; } = string.Empty;

    [CommandParameter(1, Name = "output", Description = "Output LDR file path (.jpg/.png)")]
    private string OutputLdrFileName { get; set; } = string.Empty;

    [CommandOption("factor", 'f', Description = "Scaling factor (default: 0.6)")]
    private float Factor { get; set; } = 0.6f;

    [CommandOption("gamma", 'g', Description = "Gamma correction value (default: 1.0)")]
    private float Gamma { get; set; } = 1.0f;

    public ValueTask ExecuteAsync(IConsole console)
    {
        // Here you would invoke your actual image conversion logic
        console.Output.WriteLine($"Converting {InputPfmFileName} to {OutputLdrFileName} with factor {Factor}, gamma {Gamma}");
        return default;
    }
}