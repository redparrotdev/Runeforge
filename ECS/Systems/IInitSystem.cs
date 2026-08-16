namespace ECS.Systems;

public interface IInitSystem : IDisposable
{
    World World { get; set; }

    void Init();
}
