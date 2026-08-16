namespace ECS.Structure;

public interface IComponentsBag<T> : IDisposable
    where T : class
{
    void AddFor(Entity entity, T component);
    T RemoveFor(Entity entity); 
    T GetFor(Entity entity);
    bool HaveFor(Entity entity);
}
