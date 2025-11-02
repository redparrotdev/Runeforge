using Engine.Physics.Colliders;
using Microsoft.Xna.Framework;

namespace Engine.Physics;

public static class CollisionDetector
{
    public static bool DetectCollision(Collider a, Collider b)
    {
        var isOneInactive = !a.IsActive || !b.IsActive;
        if (isOneInactive) return false;

        var collide = (a, b) switch
        {
            (CircleCollider circleA, CircleCollider circleB) => CircleToCircle(circleA, circleB),
            (BoxCollider boxA, BoxCollider boxB) => BoxToBox(boxA, boxB),
            (CircleCollider circle, BoxCollider box) => CircleToBox(circle, box),
            (BoxCollider box, CircleCollider circle) => CircleToBox(circle, box),
            _ => false
        };

        return collide;
    }

    public static bool CircleToCircle(CircleCollider a, CircleCollider b)
    {
        float distanceSquared = Vector2.DistanceSquared(a.Position, b.Position);
        float radiusSum = a.Radius + b.Radius;
        return distanceSquared <= radiusSum * radiusSum;
    }

    public static bool BoxToBox(BoxCollider a, BoxCollider b)
    {
        return !(a.Right < b.Left ||
                 a.Left > b.Right ||
                 a.Bottom < b.Top ||
                 a.Top > b.Bottom);
    }

    public static bool CircleToBox(CircleCollider circle, BoxCollider box)
    {
        float closestX = MathHelper.Clamp(circle.Position.X, box.Left, box.Right);
        float closestY = MathHelper.Clamp(circle.Position.Y, box.Top, box.Bottom);
        float distanceX = circle.Position.X - closestX;
        float distanceY = circle.Position.Y - closestY;
        float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
        return distanceSquared <= (circle.Radius * circle.Radius);
    }
}
