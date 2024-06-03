namespace CodeBase.Infrastructure.Services.Player {
  public class PlayerHealth {
    public PlayerHealth(int health) {
      Current = health;
    }

    public int TakeDamage() => --Current;
    public bool IsAlive => Current > 0;
    public int Current { get; private set; }
  }
}