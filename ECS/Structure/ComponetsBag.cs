namespace ECS.Structure;

internal sealed class ComponetsBag<T> : IComponentsBag<T>, IInternalComponentBag
    where T : class
{
    private readonly Dictionary<ulong, T> _componentsLookup = new(capacity: 256);

    public readonly Type ComponentType = typeof(T);

    public void AddFor(Entity entity, T component)
    {
        _componentsLookup.Add(entity.Id, component);
    }

    public T RemoveFor(Entity entity)
    {
        if (!HaveFor(entity)) return default!;

        var component = GetFor(entity);
        _componentsLookup.Remove(entity.Id);

        return component;
    }

    void IInternalComponentBag.RemoveComponent(Entity entity)
    {
        RemoveFor(entity);
    }

    public T GetFor(Entity entity)
    {
        if (!HaveFor(entity)) return default!;

        return _componentsLookup[entity.Id];
    }

    public bool HaveFor(Entity entity)
    {
        return _componentsLookup.ContainsKey(entity.Id);
    }

    public void Dispose()
    {
        _componentsLookup.Clear();
    }
}
