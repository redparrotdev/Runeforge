using Engine.Abstractions.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Graphics;

public class Sprite : ISprite
{
    public virtual Texture2D Texture { get; }

    public Color Color { get; set; } = Color.White;
    public float Rotation { get; set; } = 0.0f;
    public Vector2 Scale { get; set; } = Vector2.One;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public SpriteEffects Effect { get; set; } = SpriteEffects.None;
    public float LayerDepth { get; set; } = 0.0f;

    public virtual float Width => Texture.Width * Scale.X;
    public virtual float Height => Texture.Height * Scale.Y;

    public Sprite(Texture2D texture)
    {
        Texture = texture;
    }

    public Sprite SetOrigin(Vector2 origin)
    {
        Origin = origin;
        return this;
    }

    public Sprite CenterOrigin()
    {
        return SetOrigin(new Vector2(Texture.Width / 2f, Texture.Height / 2f));
    }

    public virtual void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        spriteBatch.Draw(
            Texture,
            position,
            null,
            Color,
            Rotation,
            Origin,
            Scale,
            Effect,
            LayerDepth
        );
    }
}
