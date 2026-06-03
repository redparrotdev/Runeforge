using Microsoft.Xna.Framework;

namespace Engine.Coroutines;

public interface IYieldInstruction
{
    bool ShouldWait(GameTime gameTime);
}
