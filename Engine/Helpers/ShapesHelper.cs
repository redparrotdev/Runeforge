using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Helpers;

public static class ShapesHelper
{
    public static Texture2D Rectangle(GraphicsDevice graphicsDevice, int width, int height, Color color)
    {
        var texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = color;
        }
        texture.SetData(data);
        return texture;
    }

    public static Texture2D Circle(GraphicsDevice graphicsDevice, int radius, Color color)
    {
        int diameter = radius * 2;
        var texture = new Texture2D(graphicsDevice, diameter, diameter);
        Color[] data = new Color[diameter * diameter];
        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                int index = x + y * diameter;
                Vector2 position = new Vector2(x - radius, y - radius);
                if (position.Length() <= radius)
                {
                    data[index] = color;
                }
                else
                {
                    data[index] = Color.Transparent;
                }
            }
        }
        texture.SetData(data);
        return texture;
    }

    public static Texture2D OutlinedRectangle(GraphicsDevice graphicsDevice, int width, int height, int thickness, Color color)
    {
        var texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = x + y * width;
                if (x < thickness || x >= width - thickness || y < thickness || y >= height - thickness)
                {
                    data[index] = color;
                }
                else
                {
                    data[index] = Color.Transparent;
                }
            }
        }
        texture.SetData(data);
        return texture;
    }

    public static Texture2D OutlinedCircle(GraphicsDevice graphicsDevice, int radius, int thickness, Color color)
    {
        int diameter = radius * 2;
        var texture = new Texture2D(graphicsDevice, diameter, diameter);
        Color[] data = new Color[diameter * diameter];
        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                int index = x + y * diameter;
                Vector2 position = new Vector2(x - radius, y - radius);
                float length = position.Length();
                if (length <= radius && length >= radius - thickness)
                {
                    data[index] = color;
                }
                else
                {
                    data[index] = Color.Transparent;
                }
            }
        }
        texture.SetData(data);
        return texture;
    }
}
