namespace CodeBase.Infrastructure.Services.Player {
  public class Currency {
    public Currency(int initialAmount) {
      Current = initialAmount;
    }

    public void AddCurrency(int value) => Current += value;

    public bool SpendCurrency(int value) {
      if (Current - value < 0) {
        return false;
      }

      Current -= value;
      return true;
    }

    public int Current { get; private set; }
  }
}