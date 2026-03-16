using Engine.Abstractions;
using Engine.ECS.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Engine.ECS;

public class Entity : IUpdate, IDraw
{
    public readonly string Name;

    public ComponentsList Components { get; private set; }
    public BitArray ComponentsSignature => Components.Signature;

    public Scene Scene { get; private set; }

    private bool _isAlive = true;
    public bool IsAlive => _isAlive;

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

    private bool _isVisible = true;
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible == value) return;

            _isVisible = value;
            OnVisibleChanged?.Invoke(this);
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
    public event Action<IDraw> OnVisibleChanged;
    public event Action<IUpdate> OnUpdateOrderChanged;
    public event Action<IDraw> OnDrawOrderChanged;
    public event Action<Entity> OnEntityKilled;

    public Vector2 Position { get; set; }

    public Entity(string name = null) : this(name, Vector2.Zero)
    { }

    public Entity(Vector2 position) : this(null, position)
    { }

    public Entity(string name, Vector2 position)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? Guid.NewGuid().ToString()
            : name;

        Position = position;

        Components = new(this);
    }

    public virtual void OnAddedToScene(Scene scene)
    {
        Scene = scene;
    }

    public virtual void OnRemovedFromScene(Scene scene)
    {
        Scene = null;
    }

    public void Update(GameTime gameTime)
    {
        Components.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        Components.Draw(spriteBatch, gameTime);
    }

    public void Kill()
    {
        _isAlive = false;
        _isActive = false;
        _isVisible = false;

        OnEntityKilled?.Invoke(this);
    }

    public Entity AddComponent(Component component)
    {
        Components.Add(component);

        return this;
    }

    public Entity RemoveComponent(Component component)
    {
        Components.Remove(component);

        return this;
    }

    public bool HasComponent<T>() where T : Component
    {
        return Components.Has<T>();
    }

    public T GetComponent<T>() where T : Component
    {
        return Components.Get<T>();
    }

    public IEnumerable<T> GetAllComponents<T>() where T : Component
    {
        return Components.GetAll<T>();
    }
}
