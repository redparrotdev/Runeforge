using Engine.ECS;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Components.Graphics;

public class SpriteComponent : DrawComponent
{
    public Sprite Sprite { get; set; }

    public Color Color { get; set; } = Color.White;
    public float Rotation { get; set; } = 0f;
    public Vector2 Scale { get; set; } = Vector2.One;
    public SpriteEffects SpriteEffect { get; set; } = SpriteEffects.None;
    public float LayerDepth { get; set; } = 0f;

    public Vector2 Origin
    {
        get => Sprite.Origin;
        set => Sprite.Origin = value;
    }

    public virtual Vector2 Position => Entity.Position;

    public virtual float Width => Sprite.Texture.Width * Scale.X;
    public virtual float Height => Sprite.Texture.Height * Scale.Y;

    public SpriteComponent(Sprite sprite)
    {
        Sprite = sprite;
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        spriteBatch.Draw(
            Sprite
            , Position
            , Sprite.SourceRectangle
            , Color
            , Rotation
            , Origin
            , Scale
            , SpriteEffect
            , LayerDepth);
    }
}
