using Microsoft.Xna.Framework;

namespace Engine.Coroutines.YieldInstructions;

public sealed class WaitForSeconds : IYieldInstruction
{
    private float _timeToWait;

    public WaitForSeconds(float time)
    {
        _timeToWait = time;
    }

    public bool ShouldWait(GameTime gameTime)
    {
        _timeToWait -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        return _timeToWait > 0f;
    }
}
