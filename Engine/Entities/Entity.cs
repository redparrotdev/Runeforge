using Engine.Abstractions.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Entities;

public abstract class Entity : IUpdate, IDraw
{
    public virtual string Name { get; }

    public bool IsAlive => _isAlive;
    private bool _isAlive = true;

    public event Action<Entity> OnEntityKilled;

    public event Action<IUpdate> OnActiveChanged;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                OnActiveChanged?.Invoke(this);
            }
        }
    }
    private bool _isActive = true;

    public event Action<IDraw> OnVisibilityChanged;
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                OnVisibilityChanged?.Invoke(this);
            }
        }
    }
    private bool _isVisible = true;

    protected Entity(string name)
    {
        Name = name;
    }

    public virtual void Load() { }

    public virtual void Unload() { }

    public void Kill()
    {
        _isAlive = false;
        OnEntityKilled?.Invoke(this);
    }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime) { }
}
