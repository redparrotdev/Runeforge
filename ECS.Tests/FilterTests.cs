using ECS.Tests.TestComponents;

namespace ECS.Tests;

public sealed class FilterTests
{
    [Fact]
    public void Filter_ReturnsValidEntities()
    {
        var world = new World();

        var validEntity = world.CreateEntity();
        world.CreateEntity();
        world.CreateEntity();

        var positionBag = world.GetComponentsBag<Position>();
        positionBag.AddFor(validEntity, new Position
        {
            X = 10,
            Y = 10,
        });

        var filter = world.Filter().With<Position>().Build();

        var foundEntity = filter.FirstOrDefault();

        Assert.Equal(validEntity, foundEntity);
    }
}
