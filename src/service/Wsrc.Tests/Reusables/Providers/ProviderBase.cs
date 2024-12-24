namespace Wsrc.Tests.Reusables.Providers;

public abstract class ProviderBase<T> where T : class
{
    protected abstract T Entity { get; }

    public List<T> CreateList(int count)
    {
        return Enumerable.Range(0, count).Select(_ => Entity).ToList();
    }

    public T Create(Action<T> overrides)
    {
        overrides(Entity);
        return Entity;
    }

    public T Create()
    {
        return Entity;
    }
}