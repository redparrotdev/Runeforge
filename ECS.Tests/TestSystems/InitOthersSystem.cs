using ECS.Systems;

namespace ECS.Tests.TestSystems;

internal sealed class InitOthersSystem : IInitSystem
{
    public World World { get; set; } = null!;

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
