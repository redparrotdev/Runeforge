using Engine.Abstractions.Querying;
using System;

namespace Engine.ECS.Querying.Constraints;

public sealed class WithComponentQueryConstraint : IQueryConstraint
{
    private readonly int _componentTypeId;

    public WithComponentQueryConstraint(int componentTypeId)
    {
        _componentTypeId = componentTypeId;
    }

    public bool Matches(Entity entity)
    {
        return entity.ComponentsSignature.Get(_componentTypeId);
    }
}
