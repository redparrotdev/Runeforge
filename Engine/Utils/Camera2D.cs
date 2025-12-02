using Engine.ViewportAdapters;
using Microsoft.Xna.Framework;
using System;

namespace Engine.Utils;

public class Camera2D
{
    private readonly ViewportAdapter _adapter;

    private bool _changed = false;

    private Matrix _matrix = Matrix.Identity;
    public Matrix Matrix
    {
        get
        {
            if (_changed)
            {
                UpdateMatrices();
            }

            return _matrix;
        }
    }

    private Matrix _inverse = Matrix.Identity;
    public Matrix Inverse
    {
        get
        {
            if (_changed)
            {
                UpdateMatrices();
            }

            return _inverse;
        }
    }

    private Vector2 _position;
    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            _changed = true;
        }
    }

    private Vector2 _origin;
    public Vector2 Origin
    {
        get => _origin;
        set
        {
            _origin = value;
            _changed = true;
        }
    }

    private float _rotation;
    public float Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            _changed = true;
        }
    }

    private Vector2 _zoom;
    public Vector2 Zoom
    {
        get => _zoom;
        set
        {
            _zoom = Vector2.Clamp(value, MinZoom, MaxZoom);
            _changed = true;
        }
    }

    public Vector2 MinZoom { get; set; } = new Vector2(0.1f);
    public Vector2 MaxZoom { get; set; } = new Vector2(2f);

    public Camera2D(ViewportAdapter adapter, Vector2 position, Vector2 origin)
    {
        _adapter = adapter;
        _position = position;
        _origin = origin;
        _rotation = 0f;
        _zoom = Vector2.One;
        UpdateMatrices();
    }

    public Camera2D(ViewportAdapter adapter, Vector2 position)
        : this(adapter, position, new Vector2(adapter.Viewport.Width * 0.5f, adapter.Viewport.Height * 0.5f))
    { }

    public Camera2D(ViewportAdapter adapter) : this(adapter, Vector2.Zero)
    { }

    public Vector2 CameraToScreen(Vector2 position)
    {
        var m = Matrix * _adapter.GetScaleMatrix();

        return Vector2.Transform(position, m);
    }

    public Vector2 ScreenToCamera(Vector2 position)
    {
        var m = Matrix * _adapter.GetScaleMatrix();

        return Vector2.Transform(position, Matrix.Invert(m));
    }

    public Rectangle GetCameraVisibleArea()
    {
        var tl = Vector2.Transform(Vector2.Zero, Inverse);
        var tr = Vector2.Transform(new Vector2(_adapter.VirtualWidth, 0f), Inverse);
        var bl = Vector2.Transform(new Vector2(0f, _adapter.VirtualHeight), Inverse);
        var br = Vector2.Transform(new Vector2(_adapter.VirtualWidth, _adapter.VirtualHeight), Inverse);

        var minX = MathF.Min(tl.X, MathF.Min(tr.X, MathF.Min(bl.X, br.X)));
        var minY = MathF.Min(tl.Y, MathF.Min(tr.Y, MathF.Min(bl.Y, br.Y)));
        var maxX = MathF.Max(tl.X, MathF.Max(tr.X, MathF.Max(bl.X, br.X)));
        var maxY = MathF.Max(tl.Y, MathF.Max(tr.Y, MathF.Max(bl.Y, br.Y)));

        var visibleArea = new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));

        return visibleArea;
    }

    private void UpdateMatrices()
    {
        _matrix = Matrix.Identity
            * Matrix.CreateTranslation(new Vector3(-_position, 0f))
            * Matrix.CreateRotationZ(_rotation)
            * Matrix.CreateScale(new Vector3(_zoom, 1f))
            * Matrix.CreateTranslation(new Vector3(_origin, 0f));

        _inverse = Matrix.Invert(_matrix);

        _changed = false;
    }
}
