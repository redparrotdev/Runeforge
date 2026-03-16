using Engine.ECS;

namespace Engine.Abstractions.Querying;

public interface IQueryConstraint
{
    bool Matches(Entity entity);
}
