namespace Engine.Coroutines;

public interface ICoroutine
{
    bool Done { get; }

    public void Stop();
}
