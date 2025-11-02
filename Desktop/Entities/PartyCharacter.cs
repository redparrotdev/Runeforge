using Engine.Abstractions.Graphics;
using Engine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktop.Entities;

public class PartyCharacter : Entity
{
    public Vector2 Position { get; set; }

    private ISprite Sprite { get; }

    public PartyCharacter(string name
        , ISprite sprite) 
        : base(name)
    {
        Sprite = sprite;
    }

    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        Sprite.Draw(spriteBatch, Position);
    }
}
