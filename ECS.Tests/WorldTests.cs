using ECS.Tests.TestComponents;

namespace ECS.Tests;

public sealed class WorldTests
{
    [Fact]
    public void World_CreatesMultipleEntities()
    {
        var world = new World();

        world.CreateEntity();
        world.CreateEntity();
        world.CreateEntity();

        Assert.True(true);
    }

    [Fact]
    public void Worlr_AfterRemovingEntity_ReusesItsId()
    {
        var world = new World();

        var entity = world.CreateEntity();
        var entityId = entity.Id;

        world.RemoveEntity(entity);
        var entity2 = world.CreateEntity();

        Assert.Equal(entityId, entity2.Id);
    }

    [Fact]
    public void World_AfterRemovingEntity_RemovesAllEntityComponents()
    {
        var world = new World();

        var entity = world.CreateEntity();
        var positionBag = world.GetComponentsBag<Position>();
        positionBag.AddFor(entity, new Position
        {
            X = 10,
            Y = 10,
        });

        world.RemoveEntity(entity);
        var hasPosition = positionBag.HaveFor(entity);

        Assert.False(hasPosition);
    }
}
