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
    private Func<bool> _placementAvailable;
    private Action _placementOnClick = delegate {  };
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

    public void RunEvents() => _monoEventsProvider.OnApplicationUpdateEvent += Update;
    public void StopEvents() => _monoEventsProvider.OnApplicationUpdateEvent -= Update;
    public void AllowFlyingBuilding() => _flyingBuildingAllowed = true;
    private void RestrictFlyingBuilding() => _flyingBuildingAllowed = false;

    public void StartPlacingContent(TileContentType type) {
      if (_flyingTileContent != null) {
        Object.Destroy(_flyingTileContent.gameObject);
      }

      _flyingTileContent = _gameFactory.Create(type);
      _placementAvailable = IsGroundPlacementPossible;
      _placementOnClick = PlaceGround;
    }
    
    public void StartPlacingContent(TowerType towerType) {
      if (_flyingTileContent != null) {
        Object.Destroy(_flyingTileContent.gameObject);
      }

      _flyingTileContent = _gameFactory.CreateTower(towerType);
      _placementAvailable = IsTowerPlacementPossible;
      _placementOnClick = PlaceTower;
    }

    private void Update() {
      if (_flyingBuildingAllowed == false) {
        return;
      }

      var ground = new Plane(Vector3.up, Vector3.zero);
      var ray = TouchRay;

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

    private bool IsTowerPlacementPossible() {
      if (Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1) == false) {
        return false;
      }

      TowerConfig towerConfig = _staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(TowerType.LTower);
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
      TowerConfig towerConfig = _staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(TowerType.LTower);
      Vector2Int[] scheme = towerConfig.BuildScheme.GetPlacementScheme();
      BoardTile[] potentialOccupied = new BoardTile[scheme.Length];

      for (int i = 0; i < scheme.Length; i++) {
        BoardTile tile = _board.GetTile(hit.point.x + scheme[i].x, hit.point.z + scheme[i].y);
        potentialOccupied[i] = tile;
      }

      _board.PlaceTower(potentialOccupied, TowerType.LTower, () => _unitSpawner.Collection.Targets);
    }

    public void RemoveTower() {
      if (Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1) == false) {
        return;
      }

      BoardTile tile = _board.GetTile(hit.point.x, hit.point.z);
      _board.RemoveTower(tile);
    }

    private bool IsGroundPlacementPossible() {
      if (Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1) == false) {
        return false;
      }

      BoardTile tile = _board.GetTile(hit.point.x, hit.point.z);
      return tile.Content.IsEmpty;
    }

    private void PlaceGround() {
      Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1);
      BoardTile tile = _board.GetTile(hit.point.x, hit.point.z);
      _board.PlaceGround(tile);
    }

    public void RemoveGround() {
      if (Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1) == false) {
        return;
      }

      BoardTile tile = _board.GetTile(hit.point.x, hit.point.z);
      _board.RemoveGround(tile);
    }

    private Ray TouchRay => _camera.ScreenPointToRay(_input.MousePosition);
  }
}