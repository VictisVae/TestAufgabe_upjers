using System;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services.Input;
using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Infrastructure.Services.StaticData.TowerData;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.BoardContent {
  public class TileContentBuilder {
    private readonly IGameFactory _gameFactory;
    private readonly IInputService _input;
    private readonly IUnitSpawner _unitSpawner;
    private readonly IStaticDataService _staticDataService;
    private readonly IMonoEventsProvider _monoEventsProvider;
    private readonly Camera _camera;
    private readonly GameBoard _board;
    private TileContent _flyingTileContent;
    private TowerType _currentTowerType = TowerType.Simple;
    private Func<bool> _placementAvailable;
    private Action _placementOnClick = delegate {};
    private bool _flyingBuildingAllowed;

    public TileContentBuilder
      (IGameFactory gameFactory, IInputService input, IUnitSpawner unitSpawner, IStaticDataService staticDataService,
        IMonoEventsProvider monoEventsProvider, GameBoard board) {
      _camera = Camera.main;
      _gameFactory = gameFactory;
      _input = input;
      _staticDataService = staticDataService;
      _monoEventsProvider = monoEventsProvider;
      _unitSpawner = unitSpawner;
      _board = board;
    }

    private void Update() {
      if (_flyingBuildingAllowed == false) {
        return;
      }

      Plane ground = new Plane(Vector3.up, Vector3.zero);
      Ray ray = TouchRay;

      if (ground.Raycast(ray, out float position)) {
        Vector3 worldPosition = ray.GetPoint(position);
        worldPosition.y += 0.1f;
        _flyingTileContent.transform.position = worldPosition;
      }

      bool isAvailable = _placementAvailable.Invoke();
      _flyingTileContent.ViewAvailable(isAvailable);

      if (isAvailable && _input.MouseButtonDown(0)) {
        _flyingTileContent.SetNormal();
        Object.Destroy(_flyingTileContent.gameObject);
        _flyingTileContent = null;
        RestrictFlyingBuilding();
        _placementOnClick();
      }
    }

    public void RunEvents() => _monoEventsProvider.OnApplicationUpdateEvent += Update;
    public void StopEvents() => _monoEventsProvider.OnApplicationUpdateEvent -= Update;
    public void AllowFlyingBuilding() => _flyingBuildingAllowed = true;

    public void StartPlacingContent(TileContentType type) {
      if (type == TileContentType.Ground) {
        if (_flyingTileContent != null) {
          Object.Destroy(_flyingTileContent.gameObject);
        }

        _flyingTileContent = _gameFactory.Create(type);
        _placementAvailable = IsTilePlacementPossible;
        _placementOnClick = PlacementOnClick(type);
        return;
      }

      _flyingBuildingAllowed = false;
      PlacementOnClick(type).Invoke();
    }

    public void StartPlacingContent(TowerType towerType) {
      if (_flyingTileContent != null) {
        Object.Destroy(_flyingTileContent.gameObject);
      }

      _currentTowerType = towerType;
      _flyingTileContent = _gameFactory.CreateTower(towerType);
      _placementAvailable = IsTowerPlacementPossible;
      _placementOnClick = PlaceTower;
    }

    public void RemoveTower() {
      if (Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1) == false) {
        return;
      }

      BoardTile tile = _board.GetTile(hit.point.x, hit.point.z);
      _board.RemoveTower(tile);
    }

    public void RemoveGround() {
      if (Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1) == false) {
        return;
      }

      BoardTile tile = _board.GetTile(hit.point.x, hit.point.z);
      _board.RemoveGround(tile);
    }

    private void RestrictFlyingBuilding() => _flyingBuildingAllowed = false;

    private Action PlacementOnClick(TileContentType type) {
      switch (type) {
        case TileContentType.Destination:
          return PlaceDestination;
        case TileContentType.Ground:
          return PlaceGround;
        case TileContentType.SpawnPoint:
          return PlaceSpawnPoint;
        case TileContentType.Empty:
        case TileContentType.Tower:
        default:
          return PlaceEmpty;
      }
    }

    private bool IsTowerPlacementPossible() {
      if (Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1) == false) {
        return false;
      }

      TowerConfig towerConfig = _staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(_currentTowerType);
      Vector2Int[] scheme = towerConfig.BuildScheme.GetPlacementScheme();
      BoardTile[] potentialOccupied = new BoardTile[scheme.Length];

      for (int i = 0; i < scheme.Length; i++) {
        BoardTile tile = _board.GetTile(hit.point.x + scheme[i].x, hit.point.z + scheme[i].y);
        potentialOccupied[i] = tile;

        if (tile == null || tile.Content.IsGround == false || tile.Content.IsOccupied) {
          return false;
        }
      }

      return true;
    }

    private void PlaceTower() {
      Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1);
      TowerConfig towerConfig = _staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(_currentTowerType);
      Vector2Int[] scheme = towerConfig.BuildScheme.GetPlacementScheme();
      BoardTile[] potentialOccupied = new BoardTile[scheme.Length];

      for (int i = 0; i < scheme.Length; i++) {
        BoardTile tile = _board.GetTile(hit.point.x + scheme[i].x, hit.point.z + scheme[i].y);
        potentialOccupied[i] = tile;
      }

      _board.PlaceTower(potentialOccupied, _currentTowerType, () => _unitSpawner.Collection.Targets);
    }

    private bool IsTilePlacementPossible() {
      if (Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1) == false) {
        return false;
      }

      BoardTile tile = _board.GetTile(hit.point.x, hit.point.z);
      return tile.Content.IsEmpty;
    }

    private void PlaceSpawnPoint() {
      if (_board.SpawnPointCount >= 5) {
        return;
      }
      
      BoardTile tile = _board.GetRandomEmptyTile();
      _board.PlaceSpawnPoint(tile);
    }

    private void PlaceDestination() {
      if (_board.DestinationPointCount >= 4) {
        return;
      }
      
      BoardTile tile = _board.GetRandomEmptyTile();
      _board.PlaceDestination(tile);
    }

    private void PlaceGround() {
      Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1);
      BoardTile tile = _board.GetTile(hit.point.x, hit.point.z);
      _board.PlaceGround(tile);
    }

    private void PlaceEmpty() {
      Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1);
      BoardTile tile = _board.GetTile(hit.point.x, hit.point.z);
      _board.PlaceEmpty(tile);
    }

    private Ray TouchRay => _camera.ScreenPointToRay(_input.MousePosition);
  }
}