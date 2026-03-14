using Engine.Debugging;
using Engine.ECS;
using Engine.Events;
using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Sandbox.Debugging;

internal sealed class DebugChangeSceneEvent : BaseEvent
{
    public Scene NewScene { get; init; }
    public bool SavePreviousSceneInNavigationStack { get; init; }
}

internal sealed class SceneSwitcherPanel : IDebugPanel, IDisposable
{
    public string Name => "Scene switcher";

    public ImGuiWindowFlags WindowFlags => ImGuiWindowFlags.None;

    private readonly List<(string Name, Func<Scene> Builder)> _sceneBuilders = [];

    private bool _savePreviousSceneInNavigation = false;

    public void AddScene(string name, Func<Scene> builder)
    {
        _sceneBuilders.Add((name, builder));
    }

    public void Update(GameTime gameTime)
    {
    }

    public void Draw(GameTime gameTime)
    {
        if (ImGui.Checkbox("Save previous scene in navigation stack", ref _savePreviousSceneInNavigation))
        {
            // Nothing for now
        }

        ImGui.Text("Scenes:");
        foreach (var (name, builder) in _sceneBuilders)
        {
            if (ImGui.Button(name))
            {
                var newScene = builder();
                EventManager.Dispatch(new DebugChangeSceneEvent
                {
                    NewScene = newScene,
                    SavePreviousSceneInNavigationStack = _savePreviousSceneInNavigation
                });
            }
        }
    }

    public void Dispose()
    {
        _sceneBuilders.Clear();
    }
}
