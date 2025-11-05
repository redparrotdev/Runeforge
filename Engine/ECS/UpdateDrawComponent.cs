using Engine.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.ECS;

public abstract class UpdateDrawComponent : Component, IUpdate, IDraw
{
    private bool _isActive = false;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value) return;

            _isActive = value;
            OnActiveChanged?.Invoke(this);
        }
    }

    private int _updateOrder = 0;
    public int UpdateOrder
    {
        get => _updateOrder;
        set
        {
            if (_updateOrder == value) return;

            _updateOrder = value;
            OnUpdateOrderChanged?.Invoke(this);
        }
    }

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

    public event Action<IUpdate> OnActiveChanged;
    public event Action<IUpdate> OnUpdateOrderChanged;
    public event Action<IDraw> OnVisibleChanged;
    public event Action<IDraw> OnDrawOrderChanged;

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
}
