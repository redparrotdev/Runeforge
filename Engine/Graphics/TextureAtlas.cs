using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Graphics;

public sealed class TextureAtlas
{
    public Texture2D Texture { get; init; }

    private readonly Dictionary<string, TextureRegion> _regions;

    public TextureAtlas(Texture2D texture, Dictionary<string, TextureRegion> regions)
    {
        Texture = texture;
        _regions = regions;
    }

    public TextureAtlas(Texture2D texture) : this(texture, [])
    {
    }

    public void AddRegion(string name, TextureRegion region)
    {
        _regions[name] = region;
    }

    public void AddRegion(string name, int x, int y, int width, int height)
    {
        AddRegion(name, new TextureRegion(Texture, x, y, width, height));
    }

    public TextureRegion GetRegion(string name)
    {
        return _regions.TryGetValue(name, out var region) ? region : null;
    }

    public void RemoveRegion(string name)
    {
        _regions.Remove(name);
    }

    public void ClearRegions()
    {
        _regions.Clear();
    }

    public Sprite CreateSprite(string name)
    {
        var region = GetRegion(name);
        Debug.Assert(region != null);

        return new Sprite(region);
    }
}
