using Engine.Debugging;
using Engine.Physics.Colliders;

namespace Engine.Components.Physics;

public class BoxColliderComponent : ColliderComponent
{
    [DebugExpose]
    public float Width
    {
        get => ((BoxCollider)Collider).Width;
        set => ((BoxCollider)Collider).Width = value;
    }

    [DebugExpose]
    public float Height
    {
        get => ((BoxCollider)Collider).Height;
        set => ((BoxCollider)Collider).Height = value;
    }

    public BoxColliderComponent(float width, float height) 
        : base(new BoxCollider(width, height))
    {
    }
}
