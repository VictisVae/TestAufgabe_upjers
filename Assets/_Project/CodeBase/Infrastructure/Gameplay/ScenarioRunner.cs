using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Infrastructure.Services.Player;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.UI;
using UnityEngine;

namespace CodeBase.Infrastructure.Gameplay {
  public class ScenarioRunner {
    private readonly IMonoEventsProvider _monoEventsProvider;
    private readonly IPlayerService _playerService;
    private readonly IUnitSpawner _unitSpawner;
    private readonly GameScenario _scenario;
    private readonly GameBoard _board;
    private readonly HUD _hud;
    private GameScenario.State _activeScenario;
    private float _spawnProgress;
    private bool _isPaused;

    public ScenarioRunner(IUnitSpawner unitSpawner, IMonoEventsProvider monoEventsProvider, IStaticDataService staticDataService, IPlayerService playerService, GameBoard board, HUD hud) {
      _unitSpawner = unitSpawner;
      _monoEventsProvider = monoEventsProvider;
      _playerService = playerService;
      _scenario = staticDataService.GetStaticData<GameScenario>();
      _board = board;
      _hud = hud;
    }

    private void Update() {
      //TODO Pause und Beschleinigung
      // if (_input.KeyDown(KeyCode.Space)) {
      //   _isPaused = !_isPaused;
      //   Time.timeScale = _isPaused ? 0.0f : 1.0f;
      // }
      _activeScenario.Progress();

      if (_activeScenario.NextWaveReady && _unitSpawner.Collection.IsEmpty) {
        WaveCompleted();
      }

      if (_playerService.IsAlive == false) {
        Debug.Log("Defeated");
      }

      if (_activeScenario.Progress() == false && _unitSpawner.Collection.IsEmpty) {
        Debug.Log("Victory");
        return;
      }

      _unitSpawner.Collection.GameUpdate();
      _board.GameUpdate();
    }

    private void WaveCompleted() {
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

      if (_activeScenario.NextWave() == false) {
        Debug.Log("Victory");
      }
    }

    public int TotalWaves => _scenario.Waves.Length;
  }
}