using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Abstractions.Logic;

public interface IDraw
{
    bool IsVisible { get; set; }

    event Action<IDraw> OnVisibilityChanged;

    void Draw(SpriteBatch spriteBatch, GameTime gameTime);
}
