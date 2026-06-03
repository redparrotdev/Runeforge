using Microsoft.Xna.Framework;
using System;

namespace Engine.Coroutines.YieldInstructions;

public sealed class WaitForCondition : IYieldInstruction
{
    private readonly Func<bool> _condition;

    public WaitForCondition(Func<bool> condition)
    {
        ArgumentNullException.ThrowIfNull(condition);
        _condition = condition;
    }

    public bool ShouldWait(GameTime gameTime)
    {
        return !_condition();
    }
}
