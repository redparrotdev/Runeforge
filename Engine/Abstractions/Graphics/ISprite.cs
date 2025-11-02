using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Abstractions.Graphics;
public interface ISprite
{
    public Texture2D Texture { get; }

    public Color Color { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; }
    public Vector2 Origin { get; set; }
    public SpriteEffects Effect { get; set; }
    public float LayerDepth { get; set; }

    public float Width { get; }
    public float Height { get; }

    public void Draw(SpriteBatch spriteBatch, Vector2 position);
}
