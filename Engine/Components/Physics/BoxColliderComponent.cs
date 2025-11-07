using Engine.Physics.Colliders;

namespace Engine.Components.Physics;

public class BoxColliderComponent : ColliderComponent
{
    public float Width
    {
        get => ((BoxCollider)Collider).Width;
        set => ((BoxCollider)Collider).Width = value;
    }

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
