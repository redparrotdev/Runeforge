using Engine.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.ECS.Utils;

public class EntitiesList : IEnumerable<Entity>
{
    public event Action<Entity> OnEntityAdded;
    public event Action<Entity> OnEntityRemoved;

    private readonly Scene _scene;

    private readonly List<Entity> _all = [];
    private readonly List<Entity> _updateOrdered = [];
    private readonly List<Entity> _drawOrdered = [];
    private readonly List<Entity> _toAdd = [];
    private readonly List<Entity> _toRemove = [];
    private readonly List<Entity> _callAdded = [];
    private readonly List<Entity> _callRemoved = [];
    private readonly Dictionary<string, Entity> _byNamesLookup = [];

    private bool _updating = false;
    private bool _collectionsDirty = false;

    public EntitiesList(Scene scene)
    {
        _scene = scene;
    }

    public void Update(GameTime gameTime)
    {
        UpdateEntitiesCollection();

        _updating = true;
        if (_updateOrdered.Count > 0)
        {
            foreach (var entity in _updateOrdered)
            {
                if (!entity.IsActive) continue;

                entity.Update(gameTime);
            }
        }
        _updating = false;
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        _updating = true;
        if (_drawOrdered.Count > 0)
        {
            foreach (var entity in _drawOrdered)
            {
                if (!entity.IsVisible) continue;

                entity.Draw(spriteBatch, gameTime);
            }
        }
        _updating = false;
    }

    public void Add(Entity entity)
    {
        Debug.Assert(entity != null);

        if (_updating)
        {
            _toAdd.Add(entity);
            return;
        }

        _all.Add(entity);
        _callAdded.Add(entity);
        _byNamesLookup[entity.Name] = entity;
        _updateOrdered.Add(entity);
        _drawOrdered.Add(entity);

        _collectionsDirty = true;

        entity.OnEntityKilled += OnEntityKilled;
        OnEntityAdded?.Invoke(entity);
    }

    public void Remove(Entity entity)
    {
        if (entity is null) return;

        if (_updating)
        {
            _toRemove.Add(entity);
            return;
        }

        _all.Remove(entity);
        _callRemoved.Add(entity);
        _updateOrdered.Remove(entity);
        _drawOrdered.Remove(entity);
        _byNamesLookup.Remove(entity.Name);

        entity.OnUpdateOrderChanged -= OnUpdateOrderChanged;
        entity.OnDrawOrderChanged -= OnDrawOrderChanged;
        entity.OnEntityKilled -= OnEntityKilled;

        OnEntityRemoved?.Invoke(entity);
    }

    public Entity FindByName(string name)
    {
        _byNamesLookup.TryGetValue(name, out var entity);

        return entity;
    }

    private void UpdateEntitiesCollection()
    {
        if (_toAdd.Count > 0)
        {
            foreach (var entityToAdd in _toAdd)
            {
                Add(entityToAdd);
            }

            _toAdd.Clear();
        }

        if (_callAdded.Count > 0)
        {
            _updating = true;
            foreach (var addedEntity in _callAdded)
            {
                addedEntity.OnAddedToScene(_scene);
            }
            _updating = false;

            _callAdded.Clear();
        }

        if (_toRemove.Count > 0)
        {
            foreach (var entityToRemove in _toRemove)
            {
                Remove(entityToRemove);
            }

            _toRemove.Clear();
        }

        if (_callRemoved.Count > 0)
        {
            _updating = true;
            foreach (var removedEntity in _callRemoved)
            {
                removedEntity.OnRemovedFromScene(_scene);
            }
            _updating = false;

            _callRemoved.Clear();
        }

        if (_collectionsDirty)
        {
            _updateOrdered.Sort((l, r) => r.UpdateOrder.CompareTo(l.UpdateOrder));
            _drawOrdered.Sort((l, r) => r.DrawOrder.CompareTo(l.DrawOrder));
        }
    }

    private void OnEntityKilled(Entity entity)
    {
        Remove(entity);
    }

    private void OnUpdateOrderChanged(IUpdate _)
    {
        _collectionsDirty = true;
    }

    private void OnDrawOrderChanged(IDraw _)
    {
        _collectionsDirty = true;
    }

    public IEnumerator<Entity> GetEnumerator()
    {
        return _all.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
