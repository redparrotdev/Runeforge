namespace Engine.ECS;

public abstract class Component
{
    public virtual void OnAddedToEntity(Entity entity)
    {
    }

    public virtual void OnRemovedFromEntity(Entity entity)
    {
    }
}
