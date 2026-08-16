using System.Collections;

namespace ECS.Structure;

public sealed class Filter : IEnumerable<Entity>, IDisposable
{
    private World _world;
    private IEnumerable<IFilterConstraint> _constraints;

    public Filter(World world, IEnumerable<IFilterConstraint> constraints)
    {
        _world = world;
        _constraints = constraints;
    }

    public IEnumerator<Entity> GetEnumerator()
    {
        var entitiesEnumerator = _world.Entities.GetEnumerator();

        while (entitiesEnumerator.MoveNext())
        {
            var entity = entitiesEnumerator.Current;
            var isValid = true;
            foreach (var constraint in _constraints)
            {
                if (!constraint.Match(entity))
                {
                    isValid = false;
                    break;
                }
            }

            if (!isValid) continue;

            yield return entity;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        _world = null!;
        foreach (var constraint in _constraints)
        {
            constraint.Dispose();
        }
        _constraints = null!;
    }
}
