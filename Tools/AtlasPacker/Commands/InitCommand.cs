using AtlasPacker.Helpers;
using System.CommandLine;
using System.Xml;
using System.Xml.Linq;

namespace AtlasPacker.Commands;

internal sealed class InitCommand : Command
{
    public const string COMMAND_NAME = "init";
    public const string COMMAND_DESCRIPTION = "Initializes sprite atlas packer with new or existing atlas.";

    public readonly Argument<string> AtlasFilePathArgument;
    public readonly Argument<int> CellWidthArgument;
    public readonly Argument<int> CellHeightArgument;

    public InitCommand() : base(COMMAND_NAME, COMMAND_DESCRIPTION)
    {
        AtlasFilePathArgument = new Argument<string>("atlas-file-path")
        {
            Description = "Path to the new or existing sprite atlas."
        };

        CellWidthArgument = new Argument<int>("atlas-cell-width")
        {
            Description = "Width of a single cell inside sprite atlas."
        };

        CellHeightArgument = new Argument<int>("atlas-cell-height")
        {
            Description = "Height of a single cell inside sprite atlas."
        };

        Add(AtlasFilePathArgument);
        Add(CellWidthArgument);
        Add(CellHeightArgument);
        SetAction(Run);
    }

    private async Task Run(ParseResult parseResult, CancellationToken ct)
    {
        var atlasPath = parseResult.GetValue(AtlasFilePathArgument)!;
        if (!File.Exists(atlasPath))
        {
            CreateEmptyAtlasFile(atlasPath);
            Console.WriteLine("Created new atlas file");
        }

        var name = Path.GetFileNameWithoutExtension(atlasPath);
        var cellWidth = parseResult.GetValue(CellWidthArgument)!;
        var cellHeight = parseResult.GetValue(CellHeightArgument)!;

        var configInfo = new AtlasInfo
        {
            Name = name,
            FilePath = atlasPath,
            CellWidth = cellWidth,
            CellHeight = cellHeight
        };

        ConfigHelper.CreateConfigFile(configInfo);

        Console.WriteLine($"Initialized atlas '{name}'. File path: {atlasPath}");

        await Task.CompletedTask;
    }

    private static void CreateEmptyAtlasFile(string path)
    {
        var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
        var root = new XElement("SpriteAtlas"
            , new XElement("Texture", string.Empty));

        doc.AddFirst(root);

        using var writer = XmlHelper.CreateWriter(path);
        doc.Save(writer);

        writer.Flush();
    }
}
