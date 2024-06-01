using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Infrastructure.States;

namespace CodeBase.Infrastructure.GameBoot {
  public class Game {
    public Game(ICoroutineHandler coroutineHandler, IMonoEventsProvider monoEventsProvider) =>
      StateMachine = new GameStateMachine(new SceneLoader(coroutineHandler), GlobalService.Container, monoEventsProvider);

    public IGameStateMachine StateMachine { get; }
  }
}