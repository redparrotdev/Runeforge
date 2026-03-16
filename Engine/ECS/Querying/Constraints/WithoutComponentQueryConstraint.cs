using Engine.Abstractions.Querying;

namespace Engine.ECS.Querying.Constraints;

public sealed class WithoutComponentQueryConstraint : IQueryConstraint
{
    private readonly int _componentTypeId;

    public WithoutComponentQueryConstraint(int componentTypeId)
    {
        _componentTypeId = componentTypeId;
    }

    public bool Matches(Entity entity)
    {
        return !entity.ComponentsSignature.Get(_componentTypeId);
    }
}
