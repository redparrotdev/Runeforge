using System.Collections;

namespace Feather.Core.Helpers;

public sealed class LookaheadEnumerator<T> : IEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public T Current { get; private set; }
    public bool HasNext { get; private set; }

    object IEnumerator.Current => Current!;

    public LookaheadEnumerator(IEnumerable<T> enumerable) : this(enumerable.GetEnumerator())
    { }

    public LookaheadEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
        Current = default!;

        HasNext = _inner.MoveNext();
        if (HasNext)
        { 
            Current = _inner.Current;
        }
    }

    public void Dispose()
    {
        if (Current is IDisposable disposable)
        {
            disposable.Dispose();
        }
        _inner.Dispose();
    }

    public bool MoveNext()
    {
        if (!HasNext) return false;

        Current = _inner.Current;
        HasNext = _inner.MoveNext();

        return true;
    }

    public void Reset()
    {
        _inner.Reset();

        Current = default!;

        HasNext = _inner.MoveNext();
        if (HasNext)
        {
            Current = _inner.Current;
        }
    }
}
