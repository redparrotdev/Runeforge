using ImGuiNET;
using Microsoft.Xna.Framework;
using System;

namespace Engine.Debugging.Panels;

public sealed class RepformancePanel : IDebugPanel
{
    public string Name => "Performance";

    public ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.None;

    private float _fps = 0f;
    private float _memory = 0f;

    public void Update(GameTime gameTime)
    {
        _fps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;
        _memory = GC.GetTotalMemory(false) / 1024f / 1024f;
    }

    public void Draw(GameTime gameTime)
    {
        ImGui.Text($"FPS: {_fps:F2}");
        ImGui.Text($"Memory: {_memory:F2} MB");
    }
}
