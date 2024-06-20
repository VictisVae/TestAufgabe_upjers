using System;
using CodeBase.Grid;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services.Input;
using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Infrastructure.Services.StaticData.TowerData;
using CodeBase.Towers;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.BoardContent {
  public class TileContentBuilder {
    private readonly IGameFactory _gameFactory;
    private readonly IInputService _input;
    private readonly IUnitSpawner _unitSpawner;
    private readonly IStaticDataService _staticDataService;
    private readonly IMonoEventsProvider _monoEventsProvider;
    private readonly Camera _camera;
    private readonly GridController _gridController;
    private FactoryObject _flyingTileContent;
    private TowerType _currentTowerType = TowerType.Simple;
    private Func<bool> _placementAvailable;
    private Action _placementOnClick = delegate {};
    private Vector3 _movementVelocity = Vector3.zero;
    private float _dampingFactor = 0.075f;
    private bool _flyingBuildingAllowed;
    private bool _readyToBePlaced;

    public TileContentBuilder
      (IGameFactory gameFactory, IInputService input, IUnitSpawner unitSpawner, IStaticDataService staticDataService,
        IMonoEventsProvider monoEventsProvider, GridController gridController) {
      _camera = Camera.main;
      _gameFactory = gameFactory;
      _input = input;
      _staticDataService = staticDataService;
      _monoEventsProvider = monoEventsProvider;
      _unitSpawner = unitSpawner;
      _gridController = gridController;
    }

    private void Update() {
      if (_flyingBuildingAllowed == false) {
        TrySellTileContent();
        return;
      }

      Plane ground = new Plane(Vector3.up, Vector3.zero);
      Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

      if (ground.Raycast(ray, out float position)) {
        if (_input.MouseButtonDown(0)) {
          _flyingTileContent.gameObject.Enable();
        }

        Vector3 worldPosition = ray.GetPoint(position);
        worldPosition.y += 0.1f;
        Vector3 currentPosition = _flyingTileContent.transform.position;

        if (Vector3.SqrMagnitude(currentPosition - worldPosition) > 0.01f) {
          currentPosition = Vector3.SmoothDamp(currentPosition, worldPosition, ref _movementVelocity, _dampingFactor);
          _flyingTileContent.transform.position = currentPosition;
        } else {
          _movementVelocity = Vector3.zero;
        }

        _readyToBePlaced = Vector3.Distance(_flyingTileContent.transform.position, worldPosition) <= 1.0f;
      }

      bool isAvailable = _placementAvailable.Invoke();
      _flyingTileContent.ViewAvailable(isAvailable);

      if (isAvailable && _readyToBePlaced && _input.MouseButtonDown(0)) {
        _flyingTileContent.SetNormal();
        _flyingTileContent.Recycle();
        _flyingTileContent = null;
        RestrictFlyingBuilding();
        _placementOnClick();
      }
    }

    private void TrySellTileContent() {
      if (_input.MouseButtonDown(0) == false) {
        return;
      }

      if (_input.HasPosition(out RaycastHit hit) == false) {
        return;
      }

      GridTile tile = _gridController.GetTile(hit.point);

      if (tile.Content.IsOccupied) {
        SellTower(tile);
        return;
      }

      if (tile.Content.IsGround) {
        SellGround(tile);
      }
    }

    public void RunEvents() => _monoEventsProvider.OnApplicationUpdateEvent += Update;
    public void StopEvents() => _monoEventsProvider.OnApplicationUpdateEvent -= Update;
    public void AllowFlyingBuilding() => _flyingBuildingAllowed = true;

    public void StartPlacingContent(TileContentType type) {
      if (type == TileContentType.Ground) {
        if (_flyingTileContent != null) {
          _flyingTileContent.Recycle();
        }

        _movementVelocity = Vector3.zero;
        _flyingTileContent = _gameFactory.Create(type);

        if (Application.isMobilePlatform) {
          _flyingTileContent.gameObject.Disable();
        }

        _placementAvailable = IsTilePlacementPossible;
        _placementOnClick = PlacementOnClick(type);
        return;
      }

      _flyingBuildingAllowed = false;
      PlacementOnClick(type).Invoke();
    }

    public void StartPlacingContent(TowerType towerType) {
      if (_flyingTileContent != null) {
        _flyingTileContent.Recycle();
      }

      _movementVelocity = Vector3.zero;
      _currentTowerType = towerType;
      _flyingTileContent = _gameFactory.CreateTower(towerType).With(x => x.ShowRadius());
      
      if (Application.isMobilePlatform) {
        _flyingTileContent.gameObject.Disable();
      }

      _placementAvailable = IsTowerPlacementPossible;
      _placementOnClick = PlaceTower;
    }

    private void SellTower(GridTile tile) => _gridController.RemoveTower(tile);
    private void SellGround(GridTile tile) => _gridController.RemoveGround(tile);
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
        default:
          return PlaceEmpty;
      }
    }

    private bool IsTowerPlacementPossible() {
      if (_input.HasPosition(out RaycastHit hit) == false) {
        return false;
      }

      TowerConfig towerConfig = _staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(_currentTowerType);
      Vector2Int[] scheme = towerConfig.BuildScheme.GetPlacementScheme();
      GridTile[] potentialOccupied = new GridTile[scheme.Length];

      for (int i = 0; i < scheme.Length; i++) {
        Vector3 worldPosition = new Vector3(hit.point.x + scheme[i].x, 0, hit.point.z + scheme[i].y);
        GridTile tile = _gridController.GetTile(worldPosition);
        potentialOccupied[i] = tile;

        if (tile == null || tile.Content.IsGround == false || tile.Content.IsOccupied) {
          return false;
        }
      }

      return true;
    }

    private void PlaceTower() {
      Vector3 mouseWorldPosition = _input.MouseWorldPosition;
      TowerConfig towerConfig = _staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(_currentTowerType);
      Vector2Int[] scheme = towerConfig.BuildScheme.GetPlacementScheme();
      GridTile[] potentialOccupied = new GridTile[scheme.Length];

      for (int i = 0; i < scheme.Length; i++) {
        Vector3 worldPosition = new Vector3(mouseWorldPosition.x + scheme[i].x, 0, mouseWorldPosition.z + scheme[i].y);
        GridTile tile = _gridController.GetTile(worldPosition);
        potentialOccupied[i] = tile;
      }

      _gridController.PlaceTower(potentialOccupied, _currentTowerType, () => _unitSpawner.Collection.Targets);
    }

    private bool IsTilePlacementPossible() {
      if (_input.HasPosition(out RaycastHit hit) == false) {
        return false;
      }

      if (_gridController.IsValidGridPosition(hit.point) == false) {
        return false;
      }

      GridTile tile = _gridController.GetTile(hit.point);
      return tile.Content.IsEmpty;
    }

    private void PlaceSpawnPoint() {
      if (_gridController.SpawnPointCount >= 5) {
        return;
      }

      GridTile tile = _gridController.GetRandomEmptyTile();
      _gridController.PlaceSpawnPoint(tile);
    }

    private void PlaceDestination() {
      if (_gridController.DestinationPointCount >= 5) {
        return;
      }

      GridTile tile = _gridController.GetRandomEmptyTile();
      _gridController.PlaceDestination(tile);
    }

    private void PlaceGround() {
      GridTile tile = _gridController.GetTile(_input.MouseWorldPosition);
      _gridController.PlaceGround(tile);
    }

    private void PlaceEmpty() {
      GridTile tile = _gridController.GetTile(_input.MouseWorldPosition);
      _gridController.PlaceEmpty(tile);
    }
  }
}