using Engine.Abstractions;
using Microsoft.Xna.Framework;
using System;

namespace Engine.ECS;

public abstract class UpdateComponent : Component, IUpdate
{
    private bool _isActive = true;
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

    public event Action<IUpdate> OnActiveChanged;
    public event Action<IUpdate> OnUpdateOrderChanged;

    public abstract void Update(GameTime gameTime);
}
