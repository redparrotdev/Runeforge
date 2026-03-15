using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics;

public class Sprite
{
    public Texture2D Texture { get; init; }
    public Rectangle SourceRectangle { get; init; }
    public int Width => SourceRectangle.Width;
    public int Height => SourceRectangle.Height;
    public readonly Vector2 Center;

    public Vector2 Origin { get; set; }

    public Sprite(Texture2D texture, Rectangle sourceRectangle, Vector2 origin)
    {
        Texture = texture;
        SourceRectangle = sourceRectangle;
        Origin = origin;
        Center = SourceRectangle.Size.ToVector2() / 2f;
    }

    public Sprite(Texture2D texture) : this(texture, texture.Bounds, texture.Bounds.Size.ToVector2() / 2f)
    { }

    public Sprite(Texture2D texture, Vector2 origin) : this(texture, texture.Bounds, origin)
    { }

    public Sprite(Texture2D texture, Rectangle sourceRectangle) : this(texture, sourceRectangle, sourceRectangle.Size.ToVector2() / 2f)
    { }

    public static implicit operator Texture2D(Sprite sprite) => sprite.Texture;
}
