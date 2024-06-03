using System;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Infrastructure.Services.StaticData.Player;

namespace CodeBase.Infrastructure.Services.Player {
  public class PlayerService : IPlayerService {
    public event Action<int> OnHealthChangedEvent = delegate {};
    private readonly PlayerHealth _playerHealth;

    public PlayerService(IStaticDataService staticDataService) {
      _playerHealth = new PlayerHealth(staticDataService.GetStaticData<PlayerStaticData>().Health);
    }

    public void TakeDamage() {
      int health = _playerHealth.TakeDamage();
      OnHealthChangedEvent(health);
    }
    
    public bool IsAlive => _playerHealth.IsAlive;
    public int Health => _playerHealth.Current;
  }
}