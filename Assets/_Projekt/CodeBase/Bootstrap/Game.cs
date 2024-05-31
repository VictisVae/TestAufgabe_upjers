using CodeBase.Factory;
using CodeBase.SceneCreation;
using CodeBase.Units;
using UnityEngine;
using Random = UnityEngine.Random;
using UnitBase = CodeBase.Units.UnitBase;

namespace CodeBase.Bootstrap {
  public class Game : MonoBehaviour {
    private readonly UnitsCollection _unitsCollection = new UnitsCollection();
    
    [SerializeField]
    private GameBoard _board;
    [SerializeField]
    private Vector2Int _size;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private TileContentFactory _contentFactory;
    [SerializeField]
    private UnitFactory _unitFactory;
    [SerializeField]
    [Range(0.1f, 10.0f)]
    private float _spawnSpeed;
    private float _spawnProgress;
    private void Start() => _board.Initialize(_size, _contentFactory);

    private void Update() {
      if (Input.GetMouseButtonDown(0)) {
        HandleTouch();
      } else if (Input.GetMouseButtonDown(1)) {
        HandleAlternativeTouch();
      }

      _spawnProgress += _spawnSpeed * Time.deltaTime;

      while (_spawnProgress >= 1.0f) {
        _spawnProgress -= 1.0f;
        SpawnUnit();
      }

      _unitsCollection.GameUpdate();
      _board.GameUpdate();
    }

    private void SpawnUnit() {
      BoardTile spawnPoint = _board.GetSpawnPoint(Random.Range(0, _board.SpawnPointCount));
      BoardTile destinationPoint = _board.GetDestinationPoints(Random.Range(0, _board.DestinationPointCount));
      UnitBase unitBase = _unitFactory.Get();

      if (unitBase is AirUnit airUnit) {
        airUnit.SpawnItOn(spawnPoint, destinationPoint);
      } else {
        unitBase.SpawnItOn(spawnPoint);
      }
      
      _unitsCollection.Add(unitBase);
    }

    private void HandleTouch() {
      BoardTile tile = _board.GetTile(TouchRay);

      if (tile == null) {
        return;
      }
      
      if (Input.GetKey(KeyCode.LeftShift)) {
        _board.ToggleTower(tile, () => _unitsCollection.Targets);
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

    private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);
  }
}