using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.Scenes;

public abstract class Scene : IDisposable
{
    private bool _disposed;

    protected readonly ContentManager Content;
    protected readonly SpriteBatch SpriteBatch;

    protected Scene(Game game)
    {
        Content = new ContentManager(game.Services, game.Content.RootDirectory);
        SpriteBatch = new SpriteBatch(game.GraphicsDevice);
    }

    public virtual void Load() { }

    public virtual void Unload()
    {
        Content.Unload();
    }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(GameTime gameTime) { }

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
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
