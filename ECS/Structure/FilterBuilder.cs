namespace ECS.Structure;

public sealed class FilterBuilder
{
    private readonly World _world;
    private readonly List<IFilterConstraint> _constraints = new(capacity: 5);

    public FilterBuilder(World world)
    {
        _world = world;
    }

    public FilterBuilder AddConstraint(IFilterConstraint constraint)
    {
        _constraints.Add(constraint);
        return this;
    }

    public FilterBuilder With<T>() where T : class
    {
        var constraint = new PredicateConstraint<T>(
            _world
            , (e, bag) => bag.HaveFor(e));

        return AddConstraint(constraint);
    }

    public FilterBuilder Without<T>() where T : class
    {
        var constraint = new PredicateConstraint<T>(
            _world
            , (e, bag) => !bag.HaveFor(e));

        return AddConstraint(constraint);
    }

    public Filter Build()
    {
        return new Filter(_world, _constraints);
    }

    private sealed class PredicateConstraint<T> : IFilterConstraint
        where T : class
    {
        private IComponentsBag<T> _bag;
        private Func<Entity, IComponentsBag<T>, bool> _predicate;

        public PredicateConstraint(World world, Func<Entity, IComponentsBag<T>, bool> predicate)
        {
            _bag = world.GetComponentsBag<T>();
            _predicate = predicate;
        }

        public bool Match(Entity entity)
        {
            return _predicate(entity, _bag);
        }

        public void Dispose()
        {
            _bag = null!;
            _predicate = null!;
        }
    }


}
