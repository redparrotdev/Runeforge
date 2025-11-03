using Engine.Abstractions.Graphics;
using Engine.Abstractions.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Desktop.GUI;

public class Panel : IDraw
{
    public Vector2 Position { get; set; }

    private ISprite _background;
    public ISprite Background
    {
        get => _background;
        set
        {
            _background = value;
            ChangeSpriteScale();
        }
    }

    private float _width;
    public float Width
    {
        get => _width;
        set
        {
            _width = value;
            ChangeSpriteScale();
        }
    }

    private float _height;
    public float Height
    {
        get => _height;
        set
        {
            _height = value;
            ChangeSpriteScale();
        }
    }

    public Rectangle Bounds => new(
        (int)Position.X
        , (int)Position.Y
        , (int)Width
        , (int)Height);

    public bool IsVisible { get; set; } = true;

    public event Action<IDraw> OnVisibilityChanged;

    public Panel(
        float width
        , float height
        , ISprite background)
    {
        _width = width;
        _height = height;
        _background = background;
        ChangeSpriteScale();
    }

    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!IsVisible) return;

        Background.Draw(spriteBatch, Position);
    }

    private void ChangeSpriteScale()
    {
        var scaleX = _width / Background.Texture.Width;
        var scaleY = _height / Background.Texture.Height;

        var scale = new Vector2(scaleX, scaleY);
        Background.Scale = scale;
    }
}
