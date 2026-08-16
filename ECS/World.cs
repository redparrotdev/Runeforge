using ECS.Structure;
using ECS.Systems;
using System.Diagnostics;

namespace ECS;

public sealed class World : IDisposable
{
    #region Entity

    private ulong _nextEntityId = 0;
    private readonly Stack<ulong> _reusableIds = new(capacity: 256);
    private readonly HashSet<Entity> _entities = new(capacity: 256);
    private readonly List<Entity> _entitiesToAdd = new(capacity: 256);
    private readonly List<Entity> _entitiesToRemove = new(capacity: 256);

    public IReadOnlyCollection<Entity> Entities => _entities;

    public Entity CreateEntity()
    {
        if (!_reusableIds.TryPop(out var id))
        {
            id = _nextEntityId++;
        }

        var entity = new Entity(id);
        AddEntityInternal(entity);

        return entity;
    }

    public void RemoveEntity(Entity entity)
    {
        RemoveEntityInternal(entity);
    }

    private void AddEntityInternal(in Entity entity)
    {
        if (IsUpdating)
        {
            _entitiesToAdd.Add(entity);
            return;
        }

        var result = _entities.Add(entity);
        Debug.Assert(result, "Attempt to add a duplicate entity.");
    }

    private void RemoveEntityInternal(in Entity entity)
    {
        if (IsUpdating)
        {
            _entitiesToRemove.Add(entity);
            return;
        }

        _reusableIds.Push(entity.Id);
        _entities.Remove(entity);

        foreach (var bag in _bagsLookup.Values)
        {
            bag.RemoveComponent(entity);
        }
    }

    #endregion

    #region Component bags

    private readonly Dictionary<Type, IInternalComponentBag> _bagsLookup = new(capacity: 256);

    public IComponentsBag<T> GetComponentsBag<T>()
        where T : class
    {
        var componentType = typeof(T);
        if (_bagsLookup.TryGetValue(componentType, out var bagObj))
        {
            return (IComponentsBag<T>)bagObj;
        }

        var newBag = new ComponetsBag<T>();
        _bagsLookup.Add(componentType, newBag);

        return newBag;
    }

    #endregion

    #region Systems

    private readonly List<IInitSystem> _allSystems = new(capacity: 256);
    private readonly SortedList<int, ISystem> _sortedSystems = new(capacity: 256);
    private readonly Queue<IInitSystem> _initializationQueue = new(capacity: 256);

    public bool IsUpdating { get; private set; }

    public void AddInitSystem<T>(T system) where T : class, IInitSystem
    {
        AddWorldSystem(system);
    }

    public void AddSystem<T>(int order, T system) where T : class, ISystem
    {
        AddWorldSystem(system);
        _sortedSystems.Add(order, system);
    }

    public void Init()
    {
        while (_initializationQueue.Count > 0)
        {
            var system = _initializationQueue.Dequeue();
            system.Init();
        }

        _initializationQueue.Clear();
    }

    public void Update(float deltaTime)
    {
        IsUpdating = true;
        foreach (var (_, system) in _sortedSystems)
        {
            system.Update(deltaTime);
            IsUpdating = false;
            UpdateEntitiesCollections();
            IsUpdating = true;
        }
        IsUpdating = false;
    }

    private void AddWorldSystem<T>(T system) where T : class, IInitSystem
    {
        system.World = this;
        _allSystems.Add(system);
        _initializationQueue.Enqueue(system);
    }

    private void UpdateEntitiesCollections()
    {
        foreach (var entity in _entitiesToAdd)
        {
            AddEntityInternal(entity);
        }

        foreach (var entity in _entitiesToRemove)
        {
            RemoveEntityInternal(entity);
        }

        _entitiesToAdd.Clear();
        _entitiesToRemove.Clear();
    }

    #endregion

    #region Filters

    public FilterBuilder Filter() => new FilterBuilder(this);

    #endregion

    #region IDisposable

    public void Dispose()
    {
        _reusableIds.Clear();
        _entities.Clear();
        _bagsLookup.Clear();

        foreach (var sys in _allSystems)
        {
            sys.Dispose();
        }
        _allSystems.Clear();

        _sortedSystems.Clear();
        _initializationQueue.Clear();
    }

    #endregion
}
