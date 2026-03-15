using AtlasPacker.Helpers;
using System.CommandLine;
using System.Xml;
using System.Xml.Linq;

namespace AtlasPacker.Commands;

internal sealed class AddSpriteCommand : Command
{
    public const string COMMAND_NAME = "addSprite";
    public const string COMMAND_DESCRIPTION = "Adds a sprite to the current sprite atlas";

    public readonly Argument<string> SpriteNameArgument;
    public readonly Argument<int> SpriteRowArgument;
    public readonly Argument<int> SpriteColumnArgument;
    public readonly Option<int?> SpriteOriginXOption;
    public readonly Option<int?> SpriteOriginYOption;

    public AddSpriteCommand() : base(COMMAND_NAME, COMMAND_DESCRIPTION)
    {
        SpriteNameArgument = new Argument<string>("sprite-name")
        {
            Description = "A name of the sprite."
        };

        SpriteRowArgument = new Argument<int>("sprite-row")
        {
            Description = "A 1 based row number."
        };

        SpriteColumnArgument = new Argument<int>("sprite-column")
        {
            Description = "A 1 based column numder."
        };

        SpriteOriginXOption = new Option<int?>("--origin-x", "-ox")
        {
            Description = "Optional x origin of a sprite.",
            DefaultValueFactory = _ => null
        };

        SpriteOriginYOption = new Option<int?>("--origin-y", "-oy")
        {
            Description = "Optional y origin of a sprite.",
            DefaultValueFactory = _ => null
        };

        Add(SpriteNameArgument);
        Add(SpriteRowArgument);
        Add(SpriteColumnArgument);
        Add(SpriteOriginXOption);
        Add(SpriteOriginYOption);
        SetAction(Run);
    }

    private async Task Run(ParseResult result, CancellationToken ct)
    {
        var config = ConfigHelper.GetConfig();

        var name = result.GetValue(SpriteNameArgument)!;
        var row = result.GetValue(SpriteRowArgument)!;
        var col = result.GetValue(SpriteColumnArgument)!;

        var x = (col - 1) * config.CellWidth;
        var y = (row - 1) * config.CellHeight;
        var originX = result.GetValue(SpriteOriginXOption);
        var originY = result.GetValue(SpriteOriginYOption);

        await AddSpriteToAtlas(config, name, x, y, ct, originX, originY);

        Console.WriteLine($"Added sprite '{name}' X:{x} Y:{y} Width:{config.CellWidth} Height:{config.CellWidth} OriginX:{originX} OriginY:{originY}");
    }

    private static async Task AddSpriteToAtlas(AtlasInfo config, string name, int x, int y, CancellationToken ct, int? originX = null, int? originY = null)
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

        var spriteNode = new XElement(Constants.SPRITE_ELEMENT_NAME);
        spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_NAME, name);
        spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_X, x);
        spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_Y, y);
        spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_WIDTH, config.CellWidth);
        spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_HEIGHT, config.CellHeight);
        spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_ORIGIN_X, originX);
        spriteNode.SetAttributeValue(Constants.SPRITE_ATTRIBUTE_ORIGIN_Y, originY);

        sprites.Add(spriteNode);

        await using var writer = XmlHelper.CreateWriter(config.FilePath);
        doc.Save(writer);

        writer.Flush();
    }
}
