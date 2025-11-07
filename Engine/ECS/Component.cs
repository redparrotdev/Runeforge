namespace Engine.ECS;

public abstract class Component
{
    public Entity Entity { get; private set; }

    public virtual void OnAddedToEntity(Entity entity)
    {
        Entity = entity;
    }

    public virtual void OnRemovedFromEntity(Entity entity)
    {
        Entity = null;
    }
}
