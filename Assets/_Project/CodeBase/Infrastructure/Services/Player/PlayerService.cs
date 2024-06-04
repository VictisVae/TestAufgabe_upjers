using System;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Infrastructure.Services.StaticData.Player;

namespace CodeBase.Infrastructure.Services.Player {
  public class PlayerService : IPlayerService {
    private readonly IStaticDataService _staticDataService;
    public event Action<int> OnHealthChangedEvent = delegate {};
    public event Action<int> OnGoldChangedEvent = delegate {};
    private Health _health;
    private Currency _gold;

    public PlayerService(IStaticDataService staticDataService) {
      _staticDataService = staticDataService;
      ResetValues();
    }

    public void ResetValues() {
      PlayerStaticData playerStaticData = _staticDataService.GetStaticData<PlayerStaticData>();
      _health = new Health(playerStaticData.Health);
      _gold = new Currency(playerStaticData.Gold);
    }

    public void TakeDamage() {
      int health = _health.TakeDamage(1);
      OnHealthChangedEvent(health);
    }

    public void AddCurrency(int value) {
      _gold.AddCurrency(value);
      OnGoldChangedEvent(_gold.Current);
    }

    public bool SpendCurrency(int value) {
      if (_gold.SpendCurrency(value)) {
        OnGoldChangedEvent(_gold.Current);
        return true;
      }

      return false;
    }

    public bool IsAlive => _health.IsAlive;
    public int Health => _health.Current;
    public int Gold => _gold.Current;
  }
}