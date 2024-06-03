using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.UI;
using UnityEngine;

namespace CodeBase.Infrastructure.Gameplay {
  public class Player {
    private readonly IMonoEventsProvider _monoEventsProvider;
    private readonly IUnitSpawner _unitSpawner;
    private readonly GameScenario _scenario;
    private readonly GameBoard _board;
    private readonly HUD _hud;
    private GameScenario.State _activeScenario;
    private float _spawnProgress;
    private bool _isPaused;

    public Player(IUnitSpawner unitSpawner, IMonoEventsProvider monoEventsProvider, IStaticDataService staticDataService, GameBoard board, HUD hud) {
      _unitSpawner = unitSpawner;
      _monoEventsProvider = monoEventsProvider;
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

      // if (_scenarioInProgress) {
      //   if (_currentPlayerHealth <= 0) {
      //     Debug.Log("Defeated");
      //     GameFinished();
      //   }
      //
      //   if (_activeScenario.Progress() == false && _unitSpawner.Collection.IsEmpty) {
      //     Debug.Log("Victory");
      //     GameFinished();
      //     return;
      //   }
      // }

      _unitSpawner.Collection.GameUpdate();
      _board.GameUpdate();
    }

    private void WaveCompleted() {
      StopEvents();
      _hud.SetNextWaveReady(RunNextWave);
    }

    public void BeginNewGame() {
      _unitSpawner.Collection.Clear();
      _board.Clear();
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
  }
}