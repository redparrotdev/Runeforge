using Engine.ECS;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Engine.Utils;

public class SceneManager
{
    protected readonly Stack<Scene> _navigationStack = [];

    public Scene CurrentScene => _navigationStack.TryPeek(out var scene) ? scene : null;

    public void SetScene(Scene scene, bool savePreviousInNavigation = true)
    {
        if (!savePreviousInNavigation && CurrentScene is { } current)
        {
            current.Unload();
        }

        scene.Load();

        _navigationStack.Push(scene);
    }

    public void GoBack()
    {
        if (_navigationStack.Count < 2) return;

        if (!_navigationStack.TryPop(out var scene)) return;

        scene.Unload();
    }

    public void ClearNavigationStack()
    {
        while (_navigationStack.TryPop(out var scene))
        {
            scene.Unload();
        }
    }

    public void Update(GameTime gameTime)
    {
        var current = CurrentScene;

        if (current is null) return;

        current.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        var current = CurrentScene;

        if (current is null) return;

        current.Draw(gameTime);
    }
}
