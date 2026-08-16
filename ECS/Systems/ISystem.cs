namespace ECS.Systems;

public interface ISystem : IInitSystem
{
    void Update(float deltaTime);
}
