using CodeBase.Grid;
using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Infrastructure.Services.Player;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.UI;

namespace CodeBase.Infrastructure.Gameplay {
  public class ScenarioRunner {
    private readonly IMonoEventsProvider _monoEventsProvider;
    private readonly IPlayerService _playerService;
    private readonly IUnitSpawner _unitSpawner;
    private readonly GameScenario _scenario;
    private readonly GridController _gridController;
    private readonly GameOverScreen _gameOverScreen;
    private readonly HUD _hud;
    private GameScenario.State _activeScenario;
    private int _wavesLeft;
    private float _spawnProgress;
    private bool _isPaused;

    public ScenarioRunner(IUnitSpawner unitSpawner, IMonoEventsProvider monoEventsProvider, IStaticDataService staticDataService, IPlayerService playerService, GridController gridController, HUD hud, GameOverScreen gameOverScreen) {
      _unitSpawner = unitSpawner;
      _monoEventsProvider = monoEventsProvider;
      _playerService = playerService;
      _scenario = staticDataService.GetStaticData<GameScenario>();
      _gridController = gridController;
      _hud = hud;
      _gameOverScreen = gameOverScreen;
      _wavesLeft = TotalWaves;
    }

    private void Update() {
      _activeScenario.Progress();

      if (_activeScenario.WaveCompleted && _unitSpawner.Collection.IsEmpty) {
        WaveCompleted();
      }

      if (_playerService.IsAlive == false) {
        _gameOverScreen.Appear(false);
        StopEvents();
        return;
      }

      if (_wavesLeft <= 0 && _unitSpawner.Collection.IsEmpty) {
        _gameOverScreen.Appear(true);
        StopEvents();
        return;
      }

      _unitSpawner.Collection.GameUpdate();
      _gridController.GameUpdate();
    }

    private void WaveCompleted() {
      _wavesLeft--;
      StopEvents();
      _hud.SetNextWaveReady(RunNextWave);
    }

    public void BeginNewGame() {
      _activeScenario = _scenario.Begin();
      RunEvents();
    }

    private void RunEvents() => _monoEventsProvider.OnApplicationUpdateEvent += Update;
    private void StopEvents() => _monoEventsProvider.OnApplicationUpdateEvent -= Update;

    private void RunNextWave() {
      RunEvents();
      _activeScenario.NextWave();
    }

    public int TotalWaves => _scenario.Waves.Length;
  }
}