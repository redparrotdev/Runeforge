using AtlasPacker.Commands;
using System.CommandLine;

namespace AtlasPacker;

internal class Program
{
    static async Task Main(string[] args)
    {
        var root = new RootCommand("Sprite atlas packer.")
        {
            new InitCommand(),
            new InfoCommand(),
            new AddSpriteCommand(),
            new AddSpritesRowCommand(),
            new CreateAnimationCommand(),
            new GenerateAnimationCommand()
        };

        var parseResult = root.Parse(args);
        await parseResult.InvokeAsync();
    }
}
