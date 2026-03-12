using Engine.Debugging;
using Engine.Physics.Colliders;

namespace Engine.Components.Physics;

public class CircleColliderComponent : ColliderComponent
{
    [DebugExpose]
    public float Radius
    {
        get => ((CircleCollider)Collider).Radius;
        set => ((CircleCollider)Collider).Radius = value;
    }

    public CircleColliderComponent(float radius) : base(new CircleCollider(radius))
    {
    }
}
