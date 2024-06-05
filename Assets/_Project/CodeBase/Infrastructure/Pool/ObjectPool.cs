using System;
using System.Collections.Concurrent;
using CodeBase.Utilities;

namespace CodeBase.Infrastructure.Pool {
  public class ObjectPool<T> where T : IPoolable {
    private readonly ConcurrentBag<T> _objects;
    private readonly Func<T> _objectGenerator;

    protected ObjectPool(Func<T> objectGenerator) {
      _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
      _objects = new ConcurrentBag<T>();
    }

    public T Get() {
      _objects.TryTake(out T item);
      item = item == null ? _objectGenerator() : item;
      return item.With(x => x.Get());
    }

    public void Return(T item) => _objects.Add(item.With(x => x.Return()));
  }
}