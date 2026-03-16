using Engine.Abstractions.Querying;
using System.Collections.Generic;

namespace Engine.ECS.Querying.Constraints;

public sealed class WithAnyComponentQueryConstraint : IQueryConstraint
{
    private readonly int[] _componentTypeIds;

    public WithAnyComponentQueryConstraint(IEnumerable<int> componentIds)
    {
        _componentTypeIds = [..componentIds];
    }

    public bool Matches(Entity entity)
    {
        for (int i = 0; i < _componentTypeIds.Length; i++)
        {
            if (entity.ComponentsSignature.Get(_componentTypeIds[i]))
                return true;
        }

        return false;
    }
}
