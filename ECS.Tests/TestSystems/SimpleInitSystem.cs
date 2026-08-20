using ECS.Systems;

namespace ECS.Tests.TestSystems;

internal sealed class SimpleInitSystem : IInitSystem
{
    private readonly HashSet<string> _resultRef;

    public SimpleInitSystem(HashSet<string> resultRef)
    {
        _resultRef = resultRef;
    }

    public World World { get; set; } = null!;

    public void Dispose()
    {
    }

    public void Init()
    {
        _resultRef.Add(nameof(SimpleInitSystem));
    }
}
