using AtlasPacker.Helpers;
using System.CommandLine;
using System.Xml.Linq;

namespace AtlasPacker.Commands;

internal sealed class AddSpritesRowCommand : Command
{
    public const string COMMAND_NAME = "addSpritesRow";
    public const string COMMAND_DESCRIPTION = "Adds a row of sprites.";

    public readonly Argument<int> RowArgument;
    public readonly Argument<int> CountArgument;
    public readonly Argument<string> PrefixArgument;
    public readonly Option<int> ColumnOption;
    public readonly Option<string> FormatOption;
    public readonly Option<int?> OriginXOption;
    public readonly Option<int?> OriginYOption;

    public AddSpritesRowCommand() : base(COMMAND_NAME, COMMAND_DESCRIPTION)
    {
        RowArgument = new Argument<int>("row")
        {
            Description = "A 1 base row number."
        };

        CountArgument = new Argument<int>("count")
        {
            Description = "Amount of images to take."
        };

        PrefixArgument = new Argument<string>("prefix")
        {
            Description = "Sprite name prefix used in final sprite name"
        };

        ColumnOption = new Option<int>("--column", "-c")
        {
            Description = "A 1 base column number to start with.",
            DefaultValueFactory = _ => 1
        };

        FormatOption = new Option<string>("--format", "-f", "-fmt")
        {
            Description = "A format string used to build final name for each sprite. {0} - for prefix, {1} - for sprite number (starting with 1).",
            DefaultValueFactory = _ => "{0}-{1}"
        };

        OriginXOption = new Option<int?>("--originX", "-ox")
        {
            Description = "X origin for all sprites in range.",
            DefaultValueFactory = _ => null
        };

        OriginYOption = new Option<int?>("--originY", "-oy")
        {
            Description = "Y origin for all sprites in range.",
            DefaultValueFactory = _ => null
        };

        Add(RowArgument);
        Add(CountArgument);
        Add(PrefixArgument);
        Add(ColumnOption);
        Add(FormatOption);
        Add(OriginXOption);
        Add(OriginYOption);

        SetAction(Run);
    }

    private async Task Run(ParseResult result, CancellationToken ct)
    {
        var config = ConfigHelper.GetConfig();

        var row = result.GetValue(RowArgument)!;
        var count = result.GetValue(CountArgument)!;
        var prefix = result.GetValue(PrefixArgument)!;
        var column = result.GetValue(ColumnOption)!;
        var format = result.GetValue(FormatOption)!;
        var originX = result.GetValue(OriginXOption);
        var originY = result.GetValue(OriginYOption);

        row -= 1;
        column -= 1;

        var y = config.CellHeight * row;

        var spritesData = new List<SpriteInfo>(count);
        for (int i = 0; i< count; i++)
        {
            var x = config.CellWidth * (i + column);
            var name = string.Format(format, prefix, i + 1);

            spritesData.Add(new SpriteInfo
            {
                Name = name,
                X = x,
                Y = y,
            });
        }

        await AddSpritesRowToAtals(config, spritesData, ct, originX, originY, commentText: prefix);

        Console.WriteLine($"Added {spritesData.Count} sprites.");
    }

    private static async Task AddSpritesRowToAtals(
        AtlasInfo config
        , IEnumerable<SpriteInfo> spritesData
        , CancellationToken ct
        , int? originX = null
        , int? originY = null
        , string? commentText = null)
    {
        XDocument doc;

        using (var reader = XmlHelper.CreateReader(config.FilePath))
        {
            doc = await XDocument.LoadAsync(reader, LoadOptions.None, ct);
        }

        var root = doc.Root
                   ?? throw new InvalidOperationException("Sprite atlas has no root element.");

        var sprites = root.Element(Constants.SPRITES_COLLECTION_NAME);
        if (sprites is null)
        {
            sprites = new XElement(Constants.SPRITES_COLLECTION_NAME);
            root.Add(sprites);
        }

        if (!string.IsNullOrWhiteSpace(commentText))
        {
            sprites.Add(new XComment(commentText));
        }

        foreach (var sprite in spritesData)
        {
            var spriteNode = new XElement(Constants.SPRITE_ELEMENT_NAME);
            spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_NAME, sprite.Name);
            spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_X, sprite.X);
            spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_Y, sprite.Y);
            spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_WIDTH, config.CellWidth);
            spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_HEIGHT, config.CellHeight);
            spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_ORIGIN_X, originX);
            spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_ORIGIN_Y, originY);

            sprites.Add(spriteNode);
        }

        await using var writer = XmlHelper.CreateWriter(config.FilePath);
        doc.Save(writer);

        writer.Flush();
    }

    private sealed class SpriteInfo
    {
        public required string Name { get; init; }
        public required int X { get; init; }
        public required int Y { get; init; }
    }
}
