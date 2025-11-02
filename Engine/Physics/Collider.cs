using Microsoft.Xna.Framework;

namespace Engine.Physics;

public abstract class Collider
{
    public Vector2 Position { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsTrigger { get; set; } = false;

    public virtual bool CollideWith(Collider other)
    {
        return CollisionDetector.DetectCollision(this, other);
    }
}
