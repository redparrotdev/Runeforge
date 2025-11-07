using Engine.ECS;
using Engine.Physics;
using Microsoft.Xna.Framework;

namespace Engine.Components.Physics;

public abstract class ColliderComponent : UpdateComponent
{
    public Collider Collider { get; }

    public Vector2 Position
    {
        get => Collider.Position;
        set => Collider.Position = value;
    }

    protected ColliderComponent(Collider collider)
    {
        Collider = collider;
    }

    public override void Update(GameTime gameTime)
    {
        Position = Entity.Position;
    }
}
