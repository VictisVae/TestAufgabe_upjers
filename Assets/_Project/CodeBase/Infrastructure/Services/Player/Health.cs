namespace CodeBase.Infrastructure.Services.Player {
  public class Health {
    public Health(int health) {
      Current = health;
    }

    public int TakeDamage(int damage) {
      if (Current - damage >= 0) {
        return Current -= damage;
      }

      return Current = 0;
    }

    public bool IsAlive => Current > 0;
    public int Current { get; private set; }
  }
}