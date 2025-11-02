using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.ViewportAdapters;

public class DefaultViewportAdapter : ViewportAdapter
{
    public override int VirtualWidth => ViewportWidth;
    public override int VirtualHeight => ViewportHeight;

    public DefaultViewportAdapter(GraphicsDevice graphicsDevice) : base(graphicsDevice)
    {
    }

    public override Matrix GetScaleMatrix()
    {
        return Matrix.Identity;
    }
}
