using System.Diagnostics.CodeAnalysis;

namespace ECS;

public readonly struct Entity
{
    public readonly ulong Id;

    public Entity(ulong id)
    {
        Id = id;
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
