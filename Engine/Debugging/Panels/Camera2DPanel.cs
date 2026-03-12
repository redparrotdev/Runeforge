using Engine.Utils;
using ImGuiNET;
using Microsoft.Xna.Framework;
using System;

namespace Engine.Debugging.Panels;

public sealed class Camera2DPanel : IDebugPanel
{
    public string Name => "Camera 2D";
    public ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.None;

    private readonly Camera2D _camera;

    private float _cameraSliderValue = 0f;

    public Camera2DPanel(Camera2D camera)
    {
        _camera = camera;
    }

    public void Update(GameTime gameTime)
    {
    }

    public void Draw(GameTime gameTime)
    {
        DrawVector2Control("Position", _camera.Position, v => _camera.Position = v);
        DrawCameraRotation();
        DrawVector2Control("Origin", _camera.Origin, v => _camera.Origin = v);
        DrawZoomSlider();
        DrawVector2Control("Zoom", _camera.Zoom, v => _camera.Zoom = v);
        DrawVector2Control("Min zoom", _camera.MinZoom, v => _camera.MinZoom = v);
        DrawVector2Control("Max zoom", _camera.MaxZoom, v => _camera.MaxZoom = v);
    }

    private void DrawCameraRotation()
    {
        var refValue = _camera.Rotation;
        if (ImGui.InputFloat("Rotation", ref refValue, 1f, 5f, null, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            _camera.Rotation = refValue;
        }
    }

    private void DrawZoomSlider()
    {
        if (ImGui.SliderFloat("Zoom slider", ref _cameraSliderValue, 0f, 1f))
        {
            var newX = MathHelper.Lerp(_camera.MinZoom.X, _camera.MaxZoom.X, _cameraSliderValue);
            var newY = MathHelper.Lerp(_camera.MinZoom.Y, _camera.MaxZoom.Y, _cameraSliderValue);

            _camera.Zoom = new Vector2(newX, newY);
        }
    }

    private static void DrawVector2Control(string label, Vector2 value, Action<Vector2> setter)
    {
        var valueRef = new System.Numerics.Vector2(value.X, value.Y);
        if (ImGui.InputFloat2(label, ref valueRef, null, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            setter(new Vector2(valueRef.X, valueRef.Y));
        }
    }
}
