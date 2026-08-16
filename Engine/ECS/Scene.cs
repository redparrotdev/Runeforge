using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.ECS;

public abstract class Scene : IDisposable
{
    private bool _disposed;

    public readonly Runeforge Engine;
    public readonly ContentManager Content;
    public readonly GraphicsDevice GraphicsDevice;
    public readonly GameServiceContainer Services;
    protected readonly SpriteBatch SpriteBatch;

    protected Scene(Runeforge engine)
    {
        Engine = engine;
        Content = new ContentManager(engine.Services, engine.Content.RootDirectory);
        GraphicsDevice = engine.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Services = engine.Services;
    }

    public virtual void Load()
    { }

    public virtual void Unload()
    {
        Dispose();
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
