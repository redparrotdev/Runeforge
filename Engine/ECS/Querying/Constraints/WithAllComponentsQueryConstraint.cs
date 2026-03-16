using Engine.Abstractions.Querying;
using System.Collections.Generic;

namespace Engine.ECS.Querying.Constraints;

public sealed class WithAllComponentsQueryConstraint : IQueryConstraint
{
    private readonly int[] _componentTypeIds;

    public WithAllComponentsQueryConstraint(IEnumerable<int> componentTypeIds)
    {
        _componentTypeIds = [..componentTypeIds];
    }

    public bool Matches(Entity entity)
    {
        for (int i = 0; i < _componentTypeIds.Length; i++)
        {
            if (!entity.ComponentsSignature.Get(_componentTypeIds[i]))
                return false;
        }

        return true;
    }
}
