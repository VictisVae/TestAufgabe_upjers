using System.Collections;
using CodeBase.Factory;
using CodeBase.Gameplay;
using CodeBase.SceneCreation;
using CodeBase.Units;
using UnityEngine;

namespace CodeBase.Bootstrap {
  public class Game : MonoBehaviour {
    private static Game _instance;

    public static void SpawnUnit(UnitType type) {
      BoardTile spawnPoint = _instance._board.GetSpawnPoint(Random.Range(0, _instance._board.SpawnPointCount));
      BoardTile destinationPoint = _instance._board.GetDestinationPoints(Random.Range(0, _instance._board.DestinationPointCount));
      UnitBase unitBase = _instance._factory.Get(type);

      if (unitBase is AirUnit airUnit) {
        airUnit.SpawnItOn(spawnPoint, destinationPoint);
      } else {
        unitBase.SpawnItOn(spawnPoint);
      }

      _instance._behavioursCollection.Add(unitBase);
    }

    public static void EnemyReachedDestination() => _instance._currentPlayerHealth--;
    private readonly BehavioursCollection _behavioursCollection = new BehavioursCollection();
    [SerializeField]
    private UnitFactory _factory;
    [SerializeField]
    private GameBoard _board;
    [SerializeField]
    private Vector2Int _size;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private TileContentFactory _contentFactory;
    [SerializeField]
    private GameScenario _scenario;
    [SerializeField]
    [Range(10, 100)]
    private int _startingPlayerHealth;
    [SerializeField]
    [Range(5.0f, 30.0f)]
    private float _preparationTime = 10.0f;
    private GameScenario.State _activeScenario;
    private int _currentPlayerHealth;
    private float _spawnProgress;
    private bool _scenarioInProgress;
    private bool _isPaused;
    private Coroutine _prepareRoutine;

    private void Start() {
      _board.Initialize(_size, _contentFactory);
      BeginNewGame();
    }

    private void OnEnable() => _instance = this;

    private void Update() {
      if (Input.GetKeyDown(KeyCode.Space)) {
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0.0f : 1.0f;
      }

      if (Input.GetKeyDown(KeyCode.R)) {
        BeginNewGame();
      }

      if (Input.GetMouseButtonDown(0)) {
        HandleTouch();
      } else if (Input.GetMouseButtonDown(1)) {
        HandleAlternativeTouch();
      }

      if (_scenarioInProgress) {
        if (_currentPlayerHealth <= 0) {
          Debug.Log("Defeated");
          BeginNewGame();
        }

        if (_activeScenario.Progress() == false && _behavioursCollection.IsEmpty) {
          Debug.Log("Victory");
          BeginNewGame();
          _activeScenario.Progress();
          return;
        }
      }

      _behavioursCollection.GameUpdate();
      _board.GameUpdate();
    }

    private void HandleTouch() {
      BoardTile tile = _board.GetTile(TouchRay);

      if (tile == null) {
        return;
      }

      if (Input.GetKey(KeyCode.LeftShift)) {
        _board.ToggleTower(tile, () => _behavioursCollection.Targets);
      } else {
        _board.ToggleGround(tile);
      }
    }

    private void HandleAlternativeTouch() {
      BoardTile tile = _board.GetTile(TouchRay);

      if (tile == null) {
        return;
      }

      if (Input.GetKey(KeyCode.LeftShift)) {
        _board.ToggleDestination(tile);
      } else {
        _board.ToggleSpawnPoint(tile);
      }
    }

    private void BeginNewGame() {
      _scenarioInProgress = false;

      if (_prepareRoutine != null) {
        StopCoroutine(_prepareRoutine);
      }

      _behavioursCollection.Clear();
      _board.Clear();
      _currentPlayerHealth = _startingPlayerHealth;
      _prepareRoutine = StartCoroutine(PrepareRoutine());
    }

    private IEnumerator PrepareRoutine() {
      yield return new WaitForSeconds(_preparationTime);
      _activeScenario = _scenario.Begin();
      _scenarioInProgress = true;
    }

    private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);
  }
}