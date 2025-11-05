using Engine.ECS;
using Engine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Components.Graphics;

public class SpriteComponent : DrawComponent
{
    public readonly Sprite Sprite;

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

    protected Entity Entity;

    public SpriteComponent(Sprite sprite)
    {
        Sprite = sprite;
    }

    public override void OnAddedToEntity(Entity entity)
    {
        Entity = entity;
    }

    public override void OnRemovedFromEntity(Entity entity)
    {
        Entity = null;
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
