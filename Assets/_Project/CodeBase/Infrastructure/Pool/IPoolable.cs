namespace CodeBase.Infrastructure.Pool {
  public interface IPoolable {
    public void Get();
    public void Return();
  }
}