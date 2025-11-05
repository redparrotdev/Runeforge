using Engine.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.ECS;

public abstract class DrawComponent : Component, IDraw
{
    private bool _isVisible = true;
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (IsVisible == value) return;

            _isVisible = value;
            OnVisibleChanged?.Invoke(this);
        }
    }

    private int _drawOrder = 0;
    public int DrawOrder
    {
        get => _drawOrder;
        set
        {
            if (_drawOrder == value) return;

            _drawOrder = value;
            OnDrawOrderChanged?.Invoke(this);
        }
    }

    public event Action<IDraw> OnVisibleChanged;
    public event Action<IDraw> OnDrawOrderChanged;

    public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
}
