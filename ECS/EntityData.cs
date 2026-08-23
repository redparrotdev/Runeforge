namespace ECS;

public sealed class EntityData
{
    public IReadOnlyCollection<Type> OwnedComponentTypes => _privateOwnedTypesCollection;
    private readonly HashSet<Type> _privateOwnedTypesCollection = [];

    internal EntityData()
    {
    }

    internal void AddOwnedType(Type type)
    {
        _privateOwnedTypesCollection.Add(type);
    }

    internal void RemoveOwnedType(Type type)
    {
        _privateOwnedTypesCollection.Remove(type);
    }

    internal void Clear()
    {
        _privateOwnedTypesCollection.Clear();
    }
}
