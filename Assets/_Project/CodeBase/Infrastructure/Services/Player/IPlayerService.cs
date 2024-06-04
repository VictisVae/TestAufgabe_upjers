using System;

namespace CodeBase.Infrastructure.Services.Player {
  public interface IPlayerService : IService {
    void TakeDamage();
    bool IsAlive { get; }
    int Health { get; }
    int Gold { get; }
    event Action<int> OnHealthChangedEvent;
    event Action<int> OnGoldChangedEvent;
    void AddCurrency(int value);
    bool SpendCurrency(int value);
  }
}