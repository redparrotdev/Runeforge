using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.ViewportAdapters;

public abstract class ViewportAdapter
{
    public GraphicsDevice GraphicsDevice { get; }
    public Viewport Viewport => GraphicsDevice.Viewport;

    public abstract int VirtualWidth { get; }
    public abstract int VirtualHeight { get; }
    public virtual int ViewportWidth => GraphicsDevice.Viewport.Width;
    public virtual int ViewportHeight => GraphicsDevice.Viewport.Height;

    public Rectangle Bounds => new(0, 0, VirtualWidth, VirtualHeight);
    public Vector2 Center => Bounds.Center.ToVector2();

    protected ViewportAdapter(GraphicsDevice graphicsDevice)
    {
        GraphicsDevice = graphicsDevice;
    }

    public abstract Matrix GetScaleMatrix();

    public Vector2 VectorToScreenSpace(Vector2 virtualPosition)
    {
        var scaleMatrix = GetScaleMatrix();
        var inverseMatrix = Matrix.Invert(scaleMatrix);
        return Vector2.Transform(virtualPosition, inverseMatrix);
    }

    public Vector2 VectorToVirtualSpace(Vector2 screenPosition)
    {
        var scaleMatrix = GetScaleMatrix();
        return Vector2.Transform(screenPosition, scaleMatrix);
    }
}
