namespace ECS.Structure;

public interface IFilterConstraint : IDisposable
{
    bool Match(Entity entity);
}
