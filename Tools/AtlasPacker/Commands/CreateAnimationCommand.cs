using AtlasPacker.Helpers;
using System.CommandLine;
using System.Xml.Linq;

namespace AtlasPacker.Commands;

internal sealed class CreateAnimationCommand : Command
{
    public const string COMMAND_NAME = "createAnimation";
    public const string COMMAND_DESCRIPTION = "Creates animation from sprite names.";

    public readonly Argument<string> AnimationNameArgument;
    public readonly Argument<float> FrameRateArgument;
    public readonly Argument<string[]> SpritesArgument;

    public CreateAnimationCommand() : base(COMMAND_NAME, COMMAND_DESCRIPTION)
    {
        AnimationNameArgument = new Argument<string>("animation-name")
        {
            Description = "Animation name to add."
        };

        FrameRateArgument = new Argument<float>("frame-rate")
        {
            Description = "Animation frame rate"
        };

        SpritesArgument = new Argument<string[]>("sprites")
        {
            Description = "Collection of sprite names used to create animation."
        };

        Add(AnimationNameArgument);
        Add(FrameRateArgument);
        Add(SpritesArgument);

        SetAction(Run);
    }

    private async Task Run(ParseResult result, CancellationToken ct)
    {
        var config = ConfigHelper.GetConfig();

        var animationName = result.GetValue(AnimationNameArgument)!;
        var frameRate = result.GetValue(FrameRateArgument)!;
        var spriteNames = result.GetValue(SpritesArgument)!;

        await AddAnimationToAtlas(config, animationName, frameRate, spriteNames, ct);

        Console.WriteLine($"Created animation '{animationName}' with {spriteNames.Length} frames.");
    }

    private static async Task AddAnimationToAtlas(AtlasInfo config, string animationName, float frameRate, IEnumerable<string> spriteNames, CancellationToken ct)
    {
        XDocument doc;

        using (var reader = XmlHelper.CreateReader(config.FilePath))
        {
            doc = await XDocument.LoadAsync(reader, LoadOptions.None, ct);
        }

        var root = doc.Root
                   ?? throw new InvalidOperationException("Sprite atlas has no root element.");

        var animations = root.Element(Constants.ANIMATIONS_COLLECTION_NAME);
        if (animations is null)
        {
            animations = new XElement(Constants.ANIMATIONS_COLLECTION_NAME);
            root.Add(animations);
        }

        var animationNode = new XElement(Constants.ANIMATION_ELEMENT_NAME
            , new XAttribute(Constants.ANIMATION_ATTRIBUTE_NAME, animationName)
            , new XAttribute(Constants.ANIMATION_ATTRIBUTE_FPS, frameRate));

        foreach (var spriteName in spriteNames)
        {
            var frameNode = new XElement(Constants.FRAME_ELEMENT_NAME
                , new XAttribute(Constants.FRAME_ATTRIBUTE_SPRITE, spriteName));

            animationNode.Add(frameNode);
        }

        animations.Add(animationNode);

        await using var writer = XmlHelper.CreateWriter(config.FilePath);
        doc.Save(writer);

        writer.Flush();
    }
}
