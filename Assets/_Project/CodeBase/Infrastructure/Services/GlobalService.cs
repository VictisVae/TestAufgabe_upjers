namespace CodeBase.Infrastructure.Services {
  public class GlobalService {
    private static GlobalService _instance;
    public static GlobalService Container => _instance ??= new GlobalService();

    public void RegisterSingle<TService>
      (TService implementation) where TService : IService => Implementation<TService>.ServiceInstance = implementation;

    public TService GetSingle<TService>() where TService : IService => Implementation<TService>.ServiceInstance;

    private class Implementation<TService> where TService : IService {
      public static TService ServiceInstance;
    }
  }
}