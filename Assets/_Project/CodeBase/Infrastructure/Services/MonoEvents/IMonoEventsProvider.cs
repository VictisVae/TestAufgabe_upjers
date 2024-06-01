using System;

namespace CodeBase.Infrastructure.Services.MonoEvents {
  public interface IMonoEventsProvider {
    event Action OnApplicationUpdateEvent;
  }
}