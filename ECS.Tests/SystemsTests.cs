using ECS.Tests.TestSystems;

namespace ECS.Tests;

public sealed class SystemsTests
{
    [Fact]
    public void System_AfterAddingToTheWorld_HasWorldRef()
    {
        var world = new World();
        var system = new SimpleInitSystem([]);

        world.AddInitSystem(system);

        Assert.True(object.ReferenceEquals(world, system.World));
    }

    [Fact]
    public void System_OnWorldInit_RunsOwnInitCode()
    {
        var world = new World();
        var resultRef = new HashSet<string>(capacity: 2);
        var system = new SimpleInitSystem(resultRef);

        world.AddInitSystem(system);
        world.Init();

        Assert.Contains(nameof(SimpleInitSystem), resultRef);
    }

    [Fact]
    public void World_HavingInitSystemThatAddsOtherInitSystem_InitializeAllOfThem()
    {
        var world = new World();
        var resultRef = new HashSet<string>(capacity: 2);
        var innerSystem = new SimpleInitSystem(resultRef);
        var outerSystem = new InitOthersSystem([innerSystem]);

        world.AddInitSystem(outerSystem);
        world.Init();

        Assert.Contains(nameof(SimpleInitSystem), resultRef);
    }
}
