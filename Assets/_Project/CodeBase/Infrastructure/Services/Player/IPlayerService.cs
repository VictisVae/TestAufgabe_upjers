using System;

namespace CodeBase.Infrastructure.Services.Player {
  public interface IPlayerService : IService {
    void TakeDamage();
    bool IsAlive { get; }
    int Health { get; }
    event Action<int> OnHealthChangedEvent;
  }
}