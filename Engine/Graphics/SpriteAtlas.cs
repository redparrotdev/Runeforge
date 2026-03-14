using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Engine.Graphics;

public sealed class SpriteAtlas
{
    public Texture2D Texture { get; init; }

    public IReadOnlyDictionary<string, Animation> Animations => _animations;

    private readonly Dictionary<string, Sprite> _sprites;
    private readonly Dictionary<string, Animation> _animations;

    public SpriteAtlas(Texture2D texture, Dictionary<string, Sprite> regions, Dictionary<string, Animation> animations)
    {
        Texture = texture;
        _sprites = regions;
        _animations = animations;
    }

    public SpriteAtlas(Texture2D texture) : this(texture, [], [])
    {
    }

    public void AddSprite(string name, Sprite sprite)
    {
        _sprites[name] = sprite;
    }

    public void AddSprite(string name, int x, int y, int width, int height)
    {
        AddSprite(name, new Sprite(Texture, new Rectangle(x, y, width, height)));
    }

    public Sprite GetSprite(string name)
    {
        return _sprites.GetValueOrDefault(name);
    }

    public void RemoveSprite(string name)
    {
        _sprites.Remove(name);
    }

    public void ClearSprites()
    {
        _sprites.Clear();
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
}
