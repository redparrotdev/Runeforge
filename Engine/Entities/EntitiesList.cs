using Engine.Abstractions.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Entities;

internal class EntitiesList : IEnumerable<Entity>
{
    private List<Entity> _entities;
    private List<Entity> _entitiesToAdd = [];

    private bool _isUpdating = false;

    public EntitiesList(IEnumerable<Entity> entities)
    {
        _entities = [.. entities];
    }

    public EntitiesList() : this([])
    {
    }

    public void Add(Entity entity)
    {
        if (_isUpdating)
        {
            _entitiesToAdd.Add(entity);
            return;
        }

        _entities.Add(entity);
    }

    public void Load()
    {
        foreach (var entity in this)
        {
            entity.Load();
        }
    }

    public void Unload()
    {
        foreach (var entity in this)
        {
            entity.Unload();
        }
    }

    public void Update(GameTime gameTime)
    {
        _isUpdating = true;
        foreach (var entity in this)
        {
            if (entity.IsActive)
            {
                entity.Update(gameTime);
            }
        }
        _isUpdating = false;

        _entities.AddRange(_entitiesToAdd);
        _entitiesToAdd = [];

        _entities = [.. _entities.Where(e => e.IsAlive)];
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        foreach (var entity in this)
        {
            if (entity.IsVisible)
            {
                entity.Draw(spriteBatch, gameTime);
            }
        }
    }

    public IEnumerator<Entity> GetEnumerator()
    {
        return _entities.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
