using Engine.Abstractions.Graphics;
using Engine.Entities;
using Engine.Physics;
using Engine.Physics.Colliders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktop.Entities;

public class PartyEvent : Entity
{
    public Vector2 Position { get; set; }

    public ISprite Sprite { get; }
    public Collider Collider { get; }

    public PartyEvent(string name
        , ISprite sprite) 
        : base(name)
    {
        Sprite = sprite;
        Collider = new BoxCollider(Sprite.Width, Sprite.Height)
        {
            Position = Position,
            IsTrigger = true
        };
    }

    public override void Update(GameTime gameTime)
    {
        Collider.Position = Position;
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        Sprite.Draw(spriteBatch, Position);
    }
}
