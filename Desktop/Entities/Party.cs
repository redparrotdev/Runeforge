using Engine.Abstractions.Graphics;
using Engine.Entities;
using Engine.Helpers.Graphics;
using Engine.Physics;
using Engine.Physics.Colliders;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Desktop.Entities;

public class Party : Entity
{
    public const int ColliderWidth = 450;
    public const int ColliderHeight = 256;

    public IReadOnlyCollection<PartyCharacter> Characters => _characters;
    private readonly List<PartyCharacter> _characters;

    public Collider Collider { get; }

    public Party(List<PartyCharacter> characters) : base("Characters party")
    {
        _characters = characters;

        var colliderPosition = CalculatePartyPosition();
        Collider = new BoxCollider(ColliderWidth, ColliderHeight)
        {
            Position = colliderPosition,
            IsTrigger = true
        };
    }

    private static Vector2 CalculatePartyPosition()
    {
        var x = 120 + ColliderWidth / 2;
        var y = Core.Viewport.Height - 80 - ColliderHeight / 2;

        return new Vector2(x, y);
    }
}
