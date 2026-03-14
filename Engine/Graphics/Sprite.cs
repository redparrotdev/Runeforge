using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics;

public class Sprite
{
    private readonly TextureRegion _textureRegion;
    public virtual TextureRegion TextureRegion => _textureRegion; 

    public Texture2D Texture => TextureRegion.Texture;
    public Rectangle SourceRectangle => TextureRegion.SourceRectange;
    public int Width => TextureRegion.Width;
    public int Height => TextureRegion.Height;
    public readonly Vector2 Center;

    public Vector2 Origin { get; set; }

    public Sprite(Texture2D texture, Rectangle sourceRectangle, Vector2 origin)
    {
        _textureRegion = new TextureRegion(texture, sourceRectangle);
        Origin = origin;
        Center = SourceRectangle.Size.ToVector2() / 2f;
    }

    public Sprite(TextureRegion textureRegion, Vector2 origin)
    {
        _textureRegion = textureRegion;
        Origin = origin;
        Center = textureRegion.SourceRectange.Size.ToVector2() / 2f;
    }

    public Sprite(TextureRegion textureRegion) : this(textureRegion, textureRegion.SourceRectange.Size.ToVector2() / 2f)
    {
    }

    public Sprite(Texture2D texture) : this(texture, texture.Bounds, texture.Bounds.Size.ToVector2() / 2f)
    { }

    public Sprite(Texture2D texture, Vector2 origin) : this(texture, texture.Bounds, origin)
    { }

    public Sprite(Texture2D texture, Rectangle sourceRectangle) : this(texture, sourceRectangle, sourceRectangle.Size.ToVector2() / 2f)
    { }

    public static implicit operator Texture2D(Sprite sprite) => sprite.Texture;
}
