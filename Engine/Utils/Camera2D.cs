using Engine.ViewportAdapters;
using Microsoft.Xna.Framework;

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
