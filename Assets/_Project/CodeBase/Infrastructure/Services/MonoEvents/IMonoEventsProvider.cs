using System;

namespace CodeBase.Infrastructure.Services.MonoEvents {
  public interface IMonoEventsProvider : IService {
    event Action OnApplicationUpdateEvent;
  }
}