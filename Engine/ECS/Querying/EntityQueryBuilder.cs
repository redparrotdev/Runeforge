using Engine.Abstractions.Querying;
using Engine.ECS.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Engine.ECS.Querying;

public sealed class EntityQueryBuilder
{
    public sealed class EntityQuery
    {
        public readonly IQueryConstraint[] Constraints;

        internal EntityQuery(IEnumerable<IQueryConstraint> constraints)
        {
            Constraints = [..constraints];
        }

        public bool Matches(Entity entity)
        {
            for (int i = 0; i < Constraints.Length; i++)
            {
                if (!Constraints[i].Matches(entity))
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<Entity> GetMatchingEntities(EntitiesList entitiesList)
        {
            foreach (var entity in entitiesList.Where(Matches))
            {
                yield return entity;
            }
        }

        public IEnumerable<Entity> GetMatchingEntities(Scene scene)
        {
            return GetMatchingEntities(scene.Entities);
        }
    }

    private readonly List<IQueryConstraint> _constraints = [];

    public EntityQueryBuilder AddConstraint(IQueryConstraint constraint)
    {
        _constraints.Add(constraint);
        return this;
    }

    public EntityQuery Build()
    {
        return new EntityQuery(_constraints);
    }
}
