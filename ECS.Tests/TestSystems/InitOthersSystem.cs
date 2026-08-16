using ECS.Systems;

namespace ECS.Tests.TestSystems;

internal class InitOthersSystem : IInitSystem
{
    public World World { get; set; }

    private readonly IEnumerable<IInitSystem> _systems;

    public InitOthersSystem(IEnumerable<IInitSystem> systems)
    {
        _systems = systems;
    }

    public void Dispose()
    {
        World = null!;
    }

    public void Init()
    {
        foreach (var system in _systems)
        {
            World.AddInitSystem(system);
        }
    }
}
