using AtlasPacker.Helpers;
using System.CommandLine;

namespace AtlasPacker.Commands;

internal sealed class InfoCommand : Command
{
    public const string COMMAND_NAME = "info";
    public const string COMMAND_DESCRIPTION = "Display current config.";

    public InfoCommand() : base(COMMAND_NAME, COMMAND_DESCRIPTION)
    {
        SetAction(Run);
    }

    private static void Run(ParseResult _)
    {
        if (!ConfigHelper.ConfigFileExists())
        {
            Console.WriteLine("No configuration is set.");
            return;
        }
        
        var config = ConfigHelper.GetConfig();

        Console.WriteLine($"Atlas name: {config.Name}");
        Console.WriteLine($"Atlas file path: {config.FilePath}");
        Console.WriteLine($"Cell width: {config.CellWidth} Cell height: {config.CellHeight}");
    }
}
