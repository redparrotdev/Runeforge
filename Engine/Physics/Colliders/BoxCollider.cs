using Microsoft.Xna.Framework;

namespace Engine.Physics.Colliders;

public class BoxCollider : Collider
{
    public float Width { get; set; }
    public float Height { get; set; }

    public Vector2 TopLeft => new(Position.X - Width / 2, Position.Y - Height / 2);
    public Vector2 TopRight => new(Position.X + Width / 2, Position.Y - Height / 2);
    public Vector2 BottomLeft => new(Position.X - Width / 2, Position.Y + Height / 2);
    public Vector2 BottomRight => new(Position.X + Width / 2, Position.Y + Height / 2);

    public float Top => Position.Y - Height / 2;
    public float Bottom => Position.Y + Height / 2;
    public float Left => Position.X - Width / 2;
    public float Right => Position.X + Width / 2;


    public BoxCollider(float width, float height)
    {
        Width = width;
        Height = height;
    }
}
