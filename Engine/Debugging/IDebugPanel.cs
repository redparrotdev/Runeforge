using Engine.Abstractions;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace Engine.Debugging;

public interface IDebugPanel
{
    string Name { get; }
    ImGuiWindowFlags WindowFlags { get; }

    void Update(GameTime gameTime);
    void Draw(GameTime gameTime);
}
