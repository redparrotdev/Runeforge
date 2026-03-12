using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.ImGuiNet;
using System.Collections.Generic;

namespace Engine.Debugging;

public sealed class DebugUI
{
    private readonly ImGuiRenderer _renderer;

    private readonly List<IDebugPanel> _panels = new();

    public DebugUI(Game game)
    {
        _renderer = new ImGuiRenderer(game);
        _renderer.RebuildFontAtlas();
    }

    public void AddPanel(IDebugPanel panel)
    {
        if (_panels.Contains(panel)) return;
        
        _panels.Add(panel);
    }

    public void RemovePanel(IDebugPanel panel)
    {
        _panels.Remove(panel);
    }

    public void Update(GameTime gameTime)
    {
        _renderer.BeginLayout(gameTime);

        foreach (var panel in _panels)
        {
            panel.Update(gameTime);
        }
    }

    public void Draw(GameTime gameTime)
    {
        foreach (var panel in _panels)
        {
            ImGui.Begin(panel.Name, panel.WindowFlags);
            panel.Draw(gameTime);
            ImGui.End();
        }

        _renderer.EndLayout();
    }
}
