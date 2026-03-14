using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Graphics;

public sealed class TextureAtlas
{
    public Texture2D Texture { get; init; }

    public IReadOnlyDictionary<string, Animation> Animations => _animations;

    private readonly Dictionary<string, TextureRegion> _regions;
    private readonly Dictionary<string, Animation> _animations;

    public TextureAtlas(Texture2D texture, Dictionary<string, TextureRegion> regions, Dictionary<string, Animation> animations)
    {
        Texture = texture;
        _regions = regions;
        _animations = animations;
    }

    public TextureAtlas(Texture2D texture) : this(texture, [], [])
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
        return _regions.GetValueOrDefault(name);
    }

    public void RemoveRegion(string name)
    {
        _regions.Remove(name);
    }

    public void ClearRegions()
    {
        _regions.Clear();
    }

    public void AddAnimation(string name, Animation animation)
    {
        _animations.Add(name, animation);
    }

    public void RemoveAnimation(string name)
    {
        _animations.Remove(name);
    }

    public Animation GetAnimation(string name)
    {
        return _animations.GetValueOrDefault(name);
    }

    public void ClearAnimations()
    {
        _animations.Clear();
    }

    public Sprite CreateSprite(string name)
    {
        var region = GetRegion(name);
        Debug.Assert(region != null);

        return new Sprite(region);
    }
}
