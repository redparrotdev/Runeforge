using Engine.Debugging;
using Engine.ECS;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Components.Graphics;

public class SpriteComponent : UpdateDrawComponent
{
    public Sprite Sprite { get; set; }

    [DebugExpose]
    public Color Color { get; set; } = Color.White;

    [DebugExpose]
    public float Rotation { get; set; } = 0f;

    [DebugExpose]
    public Vector2 Scale { get; set; } = Vector2.One;

    [DebugExpose("Sprite effect")]
    public SpriteEffects SpriteEffect { get; set; } = SpriteEffects.None;

    [DebugExpose("Layer depth")]
    public float LayerDepth { get; set; } = 0f;

    [DebugExpose]
    public Vector2 Origin
    {
        get => Sprite.Origin;
        set => Sprite.Origin = value;
    }

    [DebugExpose]
    public virtual Vector2 Position => Entity.Position;

    [DebugExpose]
    public virtual float Width => Sprite.Width * Scale.X;

    [DebugExpose]
    public virtual float Height => Sprite.Height * Scale.Y;

    public SpriteComponent(Sprite sprite)
    {
        Sprite = sprite;
    }

    public override void Update(GameTime gameTime)
    {
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
