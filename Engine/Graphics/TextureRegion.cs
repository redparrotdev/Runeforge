using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics;

public sealed class TextureRegion
{
    public Texture2D Texture { get; init; }

    public Rectangle SourceRectange { get; init; }

    public int Width => SourceRectange.Width;
    public int Height => SourceRectange.Height;
    public int X => SourceRectange.X;
    public int Y => SourceRectange.Y;

    public TextureRegion(Texture2D texture, Rectangle sourceRectangle)
    {
        Texture = texture;
        SourceRectange = sourceRectangle;
    }

    public TextureRegion(Texture2D texture) : this(texture, texture.Bounds)
    {
    }

    public TextureRegion(Texture2D texture, int x, int y, int width, int height) : this(texture, new Rectangle(x, y, width, height))
    { }
}
