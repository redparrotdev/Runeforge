using Engine.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Engine.ECS.Utils;

public class ComponentsList : IEnumerable<Component>
{
    public event Action<Component> ComponentAdded;
    public event Action<Component> ComponentRemoved;

    private readonly Entity _entity;

    private readonly List<Component> _all = [];
    private readonly List<IUpdate> _updatable = [];
    private readonly List<IDraw> _drawable = [];
    private readonly List<Component> _toAdd = [];
    private readonly List<Component> _toRemove = [];
    private readonly List<Component> _callAdded = [];
    private readonly List<Component> _callRemoved = [];

    private bool _updating = false;
    private bool _dirtyUpdatable = false;
    private bool _dirtyDrawable = false;

    internal ComponentsList(Entity entity)
    {
        _entity = entity;
    }

    public void Update(GameTime gameTime)
    {
        UpdateComponentsCollections();

        _updating = true;

        if (_updatable.Count > 0)
        {
            foreach (var component in _updatable)
            {
                if (!component.IsActive) continue;

                component.Update(gameTime);
            }
        }

        _updating = false;
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        _updating = true;
        if (_drawable.Count > 0)
        {
            foreach (var component in _drawable)
            {
                if (!component.IsVisible) continue;

                component.Draw(spriteBatch, gameTime);
            }
        }
        _updating = false;
    }

    public void Add(Component component)
    {
        Debug.Assert(component != null);

        if (_updating)
        {
            _toAdd.Add(component);
            return;
        }

        _all.Add(component);
        _callAdded.Add(component);

        if (component is IUpdate updatable)
        {
            _updatable.Add(updatable);
            updatable.OnUpdateOrderChanged += ComponentUpdateOrderChanged;
            _dirtyUpdatable = true;
        }
        if (component is IDraw drawable)
        {
            _drawable.Add(drawable);
            drawable.OnDrawOrderChanged += ComponentDrawableOrderChanged;
            _dirtyDrawable = true;
        }

        ComponentAdded?.Invoke(component);
    }

    public void Remove(Component component)
    {
        if (component is null) return;

        if (_updating)
        {
            _toRemove.Add(component);
            return;
        }

        _all.Remove(component);
        _callRemoved.Remove(component);

        if (component is IUpdate updatable)
        {
            _updatable.Remove(updatable);
            updatable.OnUpdateOrderChanged -= ComponentUpdateOrderChanged;
        }
        if (component is IDraw drawable)
        {
            _drawable.Remove(drawable);
            drawable.OnDrawOrderChanged -= ComponentDrawableOrderChanged;
        }

        ComponentRemoved?.Invoke(component);
    }

    public T Get<T>() where T : Component
    {
        var componentType = typeof(T);

        return componentType switch
        {
            IUpdate => _updatable.FirstOrDefault(c => c is T) as T,
            IDraw => _drawable.FirstOrDefault(c => c is T) as T,
            _ => _all.FirstOrDefault(c => c is T) as T,
        };
    }

    public IEnumerable<T> GetAll<T>() where T : Component
    {
        var componentType = typeof(T);

        return componentType switch
        {
            IUpdate => _updatable.OfType<T>(),
            IDraw => _drawable.OfType<T>(),
            _ => _all.OfType<T>()
        };
    }

    private void UpdateComponentsCollections()
    {
        if (_toAdd.Count > 0)
        {
            foreach (var component in _toAdd)
            {
                Add(component);
            }

            _toAdd.Clear();
        }

        if (_callAdded.Count > 0)
        {
            _updating = true;
            foreach (var component in _callAdded)
            {
                component.OnAddedToEntity(_entity);
            }
            _updating = false;

            _callAdded.Clear();
        }

        if (_toRemove.Count > 0)
        {
            foreach (var component in _toRemove)
            {
                Remove(component);
            }

            _toRemove.Clear();
        }

        if (_callRemoved.Count > 0)
        {
            _updating = true;
            foreach (var component in _callRemoved)
            {
                component.OnRemovedFromEntity(_entity);
            }
            _updating = false;

            _callRemoved.Clear();
        }

        if (_dirtyUpdatable)
        {
            _updatable.Sort((l, r) => l.UpdateOrder.CompareTo(r.UpdateOrder));
        }

        if (_dirtyDrawable)
        {
            _drawable.Sort((l, r) => l.DrawOrder.CompareTo(r.DrawOrder));
        }
    }

    private void ComponentUpdateOrderChanged(IUpdate _)
    {
        _dirtyUpdatable = true;
    }

    private void ComponentDrawableOrderChanged(IDraw _)
    {
        _dirtyDrawable = true;
    }

    public IEnumerator<Component> GetEnumerator()
    {
        return _all.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
