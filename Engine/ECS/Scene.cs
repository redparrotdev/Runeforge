using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.ECS;

public abstract class Scene : IDisposable
{
    private bool _disposed;

    public readonly ContentManager Content;
    public readonly GraphicsDevice GraphicsDevice;
    public readonly GameServiceContainer Services;
    protected readonly SpriteBatch SpriteBatch;

    protected Scene(Game game)
    {
        Content = new ContentManager(game.Services, game.Content.RootDirectory);
        GraphicsDevice = game.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Services = game.Services;
    }

    public virtual void Load()
    { }

    public virtual void Unload()
    {
        Content.Unload();
    }

    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void Draw(GameTime gameTime)
    {
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
