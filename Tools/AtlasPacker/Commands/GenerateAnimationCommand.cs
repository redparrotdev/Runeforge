using AtlasPacker.Helpers;
using System.CommandLine;
using System.Xml.Linq;

namespace AtlasPacker.Commands;

internal sealed class GenerateAnimationCommand : Command
{
    public const string COMMAND_NAME = "generateAnimation";
    public const string COMMAND_DESCRIPTION = "Generates provided amout of animation frames with defined sprite name format.";

    public readonly Argument<string> AnimationNameArgument;
    public readonly Argument<float> FrameRateArgument;
    public readonly Argument<int> CountFramesArgument;
    public readonly Argument<string> PrefixArgument;
    public readonly Option<string> FormatOption;

    public GenerateAnimationCommand() : base(COMMAND_NAME, COMMAND_DESCRIPTION)
    {
        AnimationNameArgument = new Argument<string>("animation-name")
        {
            Description = "Animation name to add"
        };

        FrameRateArgument = new Argument<float>("frame-rate")
        {
            Description = "Animation frame rate"
        };

        CountFramesArgument = new Argument<int>("frames-count")
        {
            Description = "Amount of frames to be generated"
        };

        PrefixArgument = new Argument<string>("prefix")
        {
            Description = "Sprite name prefix used to build final sprite name."
        };

        FormatOption = new Option<string>("format")
        {
            Description = "Format string used to build final sprite name. {0} - for prefix, {1} - for number.",
            DefaultValueFactory = _ => "{0}-{1}"
        };

        Add(AnimationNameArgument);
        Add(FrameRateArgument);
        Add(CountFramesArgument);
        Add(PrefixArgument);
        Add(FormatOption);

        SetAction(Run);
    }

    private async Task Run(ParseResult result, CancellationToken ct)
    {
        var config = ConfigHelper.GetConfig();

        var animationName = result.GetValue(AnimationNameArgument)!;
        var frameRate = result.GetValue(FrameRateArgument)!;
        var count = result.GetValue(CountFramesArgument)!;
        var prefix = result.GetValue(PrefixArgument)!;
        var format = result.GetValue(FormatOption)!;

        await GenerateAnimationToAtlas(config, animationName, frameRate, count, prefix, format, ct);

        Console.WriteLine($"Generate animation '{animationName}' with {count} frames.");
    }

    private static async Task GenerateAnimationToAtlas(
        AtlasInfo config
        , string animationName
        , float frameRate
        , int countFrames
        , string prefix
        , string format
        , CancellationToken ct)
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

        for (int i = 0; i <= countFrames; i++)
        {
            var spriteName = string.Format(format, prefix, i);
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
