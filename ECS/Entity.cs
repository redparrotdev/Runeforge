using System.Diagnostics.CodeAnalysis;

namespace ECS;

public sealed class Entity
{
    public readonly ulong Id;
    public readonly EntityData EntityData;
    internal Entity(ulong id)
    {
        Id = id;
        EntityData = new EntityData();
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not Entity other) return false;

        return Id == other.Id;
    }
}
