using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Infrastructure.States;
using UnityEngine;

namespace CodeBase.Infrastructure.GameBoot {
  public class GameBootstrapper : MonoBehaviour, ICoroutineHandler {
    [SerializeField]
    private MonoEventsProvider _monoEventsProviderPrefab;

    private void Awake() {
      var eventsProvider = Instantiate(_monoEventsProviderPrefab);
      var game = new Game(this, eventsProvider);
      game.StateMachine.Enter<BootstrapState>();
      DontDestroyOnLoad(this);
    }
  }
}