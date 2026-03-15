using System.Xml;

namespace AtlasPacker;

internal static class Constants
{
    public const string CONFIG_FILE_NAME = "config.json";

    public const string SPRITES_COLLECTION_NAME = "Sprites";
    public const string SPRITE_ELEMENT_NAME = "Sprite";
    public const string SPRITE_ATTRIBUTE_NAME = "name";
    public const string SPRITE_ATTRIBUTE_X = "x";
    public const string SPRITE_ATTRIBUTE_Y = "y";
    public const string SPRITE_ATTRIBUTE_WIDTH = "width";
    public const string SPRITE_ATTRIBUTE_HEIGHT = "height";
    public const string SPRITE_ATTRIBUTE_ORIGIN_X = "originX";
    public const string SPRITE_ATTRIBUTE_ORIGIN_Y = "originY";

    public const string ANIMATIONS_COLLECTION_NAME = "Animations";
    public const string ANIMATION_ELEMENT_NAME = "Animation";
    public const string ANIMATION_ATTRIBUTE_NAME = "name";
    public const string ANIMATION_ATTRIBUTE_FPS = "fps";
    public const string FRAME_ELEMENT_NAME = "Frame";
    public const string FRAME_ATTRIBUTE_SPRITE = "sprite";
}
