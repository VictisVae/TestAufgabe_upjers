namespace CodeBase.Infrastructure.Services.StaticData {
  public interface IStaticDataService : IService {
    T GetStaticData<T>() where T : EntityStaticData;
    void Initialize();
  }
}