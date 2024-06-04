using System;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Infrastructure.Services.StaticData.Player;

namespace CodeBase.Infrastructure.Services.Player {
  public class PlayerService : IPlayerService {
    public event Action<int> OnHealthChangedEvent = delegate {};
    private readonly Health _health;

    public PlayerService(IStaticDataService staticDataService) {
      _health = new Health(staticDataService.GetStaticData<PlayerStaticData>().Health);
    }

    public void TakeDamage() {
      int health = _health.TakeDamage(1);
      OnHealthChangedEvent(health);
    }
    
    public bool IsAlive => _health.IsAlive;
    public int Health => _health.Current;
  }
}