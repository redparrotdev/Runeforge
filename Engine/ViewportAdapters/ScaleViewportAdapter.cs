using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.ViewportAdapters;

public class ScaleViewportAdapter : ViewportAdapter
{
    public override int VirtualWidth { get; }
    public override int VirtualHeight { get; }
    public override int ViewportWidth => GraphicsDevice.PresentationParameters.BackBufferWidth;
    public override int ViewportHeight => GraphicsDevice.PresentationParameters.BackBufferHeight;

    public ScaleViewportAdapter(GraphicsDevice graphicsDevice
        , int width
        , int height)
        : base(graphicsDevice)
    {
        VirtualWidth = width;
        VirtualHeight = height;
    }

    public override Matrix GetScaleMatrix()
    {
        var scaleX = (float)ViewportWidth / VirtualWidth;
        var scaleY = (float)ViewportHeight / VirtualHeight;

        return Matrix.CreateScale(scaleX, scaleY, 1f);
    }
}
