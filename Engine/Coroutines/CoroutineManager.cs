using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Engine.Coroutines;

public static class CoroutineManager
{
    private static readonly List<CoroutineInternal> _coroutines = [];
    private static readonly List<CoroutineInternal> _coroutinesToAdd = [];
    private static bool _isUpdating;

    public static ICoroutine CreateCoroutine(IEnumerator routine)
    {
        var coroutine = new CoroutineInternal(routine);
        
        if (_isUpdating)
        {
            _coroutinesToAdd.Add(coroutine);
        }
        else
        {
            _coroutines.Add(coroutine);
        }

        return coroutine;
    }

    public static void Update(GameTime gameTime)
    {
        _isUpdating = true;
        foreach (var coroutine in _coroutines)
        {
            coroutine.Update(gameTime);

            if (coroutine.Done)
            {
                continue;
            }

            _coroutinesToAdd.Add(coroutine);
        }

        _coroutines.Clear();
        _coroutines.AddRange(_coroutinesToAdd);
        _coroutinesToAdd.Clear();
        _isUpdating = false;
    }
}

public static class Coroutine
{
    public static ICoroutine Start(IEnumerator routine)
    {
        return CoroutineManager.CreateCoroutine(routine);
    }
}
