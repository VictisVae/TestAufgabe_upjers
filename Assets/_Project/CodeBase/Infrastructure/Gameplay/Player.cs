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
    private readonly int _startingPlayerHealth = 100;
    private GameScenario.State _activeScenario;
    private int _currentPlayerHealth;
    private float _spawnProgress;
    private bool _scenarioInProgress;
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

      if (_scenarioInProgress) {
        if (_currentPlayerHealth <= 0) {
          Debug.Log("Defeated");
          _scenarioInProgress = false;
        }

        if (_activeScenario.Progress() == false && _unitSpawner.Collection.IsEmpty) {
          Debug.Log("Victory");
          _scenarioInProgress = false;
          _activeScenario.Progress();
          return;
        }
      }

      _unitSpawner.Collection.GameUpdate();
      _board.GameUpdate();
    }

    public void RunEvents() => _monoEventsProvider.OnApplicationUpdateEvent += Update;
    public void StopEvents() => _monoEventsProvider.OnApplicationUpdateEvent -= Update;

    public void BeginNewGame() {
      _unitSpawner.Collection.Clear();
      _board.Clear();
      _currentPlayerHealth = _startingPlayerHealth;
      _activeScenario = _scenario.Begin();
      _scenarioInProgress = true;
    }
  }
}