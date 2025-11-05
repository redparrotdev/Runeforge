using Engine.ECS.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.ECS;

public abstract class Scene : IDisposable
{
    private bool _disposed;

    protected readonly EntitiesList Entities;
    protected readonly ContentManager Content;
    protected readonly GraphicsDevice GraphicsDevice;
    protected readonly SpriteBatch SpriteBatch;
    protected readonly GameServiceContainer Services;

    protected Scene(Game game)
    {
        Entities = new(this);
        Content = new ContentManager(game.Services, game.Content.RootDirectory);
        GraphicsDevice = game.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }

    public virtual void Load()
    { }

    public virtual void Unload()
    {
        Content.Unload();
    }

    public virtual void Update(GameTime gameTime)
    {
        Entities.Update(gameTime);
    }

    public virtual void Draw(GameTime gameTime)
    {
        Entities.Draw(SpriteBatch, gameTime);
    }

    public void AddEntity(Entity entity)
    {
        Entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        Entities.Remove(entity);
    }

    public Entity FindEntityByName(string name)
    {
        return Entities.FindByName(name);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (!disposing) return;

        Unload();
        Content.Dispose();
        SpriteBatch.Dispose();

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
