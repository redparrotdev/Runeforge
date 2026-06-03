using Engine.Abstractions;
using System.Collections.Generic;

namespace Engine.Utils;

public static class Pool<T> where T : IPoolable, new()
{
    private static readonly Stack<T> _pool = [];

    public static T Rent()
    {
        if (_pool.Count > 0) return _pool.Pop();

        return new T();
    }

    public static void Return(T item)
    {
        item.Release();
        _pool.Push(item);
    }
}
