namespace CodeBase.Infrastructure.Services.Random {
  public class RandomService : IRandomService {
    public int Range(int min, int max) => UnityEngine.Random.Range(min, max);
  }
}