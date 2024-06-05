namespace CodeBase.Infrastructure.Services.Random {
  public interface IRandomService : IService {
    public int Range(int min, int max);
  }
}