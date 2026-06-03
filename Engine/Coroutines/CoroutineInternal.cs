using Engine.Abstractions;
using Engine.Utils;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Engine.Coroutines;

internal class CoroutineInternal : ICoroutine
{
    public bool Done { get; private set; }

    private IEnumerator _routine;
    private CoroutineInternal _innerCoroutine;
    private IYieldInstruction _current;

    public CoroutineInternal(IEnumerator routine)
    {
        _routine = routine;
        _current = null;
        _innerCoroutine = null;
        Done = false;
    }

    public void Stop()
    {
        Complete();
    }

    public void Update(GameTime gameTime)
    {
        if (Done) return;

        if (_innerCoroutine is not null)
        {
            if (!_innerCoroutine.Done) return;

            ReleaseInnerCoroutine();
        }

        if (_current is not null)
        {
            if (_current.ShouldWait(gameTime))
            {
                return;
            }

            ReleaseYeildInstruction();
        }

        if (!_routine.MoveNext())
        {
            Complete();
            return;
        }

        if (_routine.Current is IYieldInstruction instruction)
        {
            _current = instruction;
        }
        else if (_routine.Current is CoroutineInternal innerCoroutine)
        {
            _innerCoroutine = innerCoroutine;
        }
    }

    private void Complete()
    {
        Done = true;
        _routine = null;
        ReleaseInnerCoroutine();
        ReleaseYeildInstruction();
    }

    private void ReleaseInnerCoroutine()
    {
        _innerCoroutine?.Stop();
        _innerCoroutine = null;
    }

    private void ReleaseYeildInstruction()
    {
        _current = null;
    }
}
