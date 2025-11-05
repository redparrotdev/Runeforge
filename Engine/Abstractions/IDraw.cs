using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Abstractions;

public interface IDraw
{
    bool IsVisible { get; set; }
    int DrawOrder { get; set; }

    event Action<IDraw> OnVisibleChanged;
    event Action<IDraw> OnDrawOrderChanged;

    void Draw(SpriteBatch spriteBatch, GameTime gameTime);
}
