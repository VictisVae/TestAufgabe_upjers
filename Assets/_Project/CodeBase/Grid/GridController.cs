using System;
using System.Collections.Generic;
using CodeBase.BoardContent;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.Player;
using CodeBase.Infrastructure.Services.Random;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Infrastructure.Services.StaticData.BoardData;
using CodeBase.Infrastructure.Services.StaticData.TileContentData;
using CodeBase.Infrastructure.Services.StaticData.TowerData;
using CodeBase.Towers;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Grid {
  public class GridController {
    private readonly IGameFactory _factory;
    private readonly IStaticDataService _staticDataService;
    private readonly IPlayerService _playerService;
    private readonly IRandomService _randomService;
    private readonly Queue<GridTile> _searchFrontier = new Queue<GridTile>();
    private readonly List<GridTile> _spawnPoints = new List<GridTile>();
    private readonly List<GridTile> _destinationPoints = new List<GridTile>();
    private readonly List<ITileContent> _contentToUpdate = new List<ITileContent>();
    private GridSystem _gridSystem;
    private GameGridView _gridView;

    public GridController(IGameFactory factory, IStaticDataService staticDataService, IPlayerService playerService, IRandomService randomService) {
      _factory = factory;
      _staticDataService = staticDataService;
      _playerService = playerService;
      _randomService = randomService;
    }

    public void GameUpdate() {
      foreach (ITileContent tileContent in _contentToUpdate) {
        tileContent.GameUpdate();
      }
    }

    public void PlaceDestination(GridTile tile, bool isSceneRunPlacement = false) {
      if (tile.NeighborIsSpawnOrDestination) {
        PlaceDestination(GetRandomEmptyTile());
        return;
      }

      SetTileDestination(tile);
      FindPathsSuccessful();

      if (_destinationPoints.Contains(tile) == false) {
        _destinationPoints.Add(tile);
      }

      if (isSceneRunPlacement) {
        return;
      }

      TileContentConfig config = _staticDataService.GetStaticData<TileContentStorage>().GetTileConfig(TileContentType.SpawnPoint);
      _playerService.AddCurrency(config.GoldValue);
    }

    public void PlaceGround(GridTile tile) {
      SetTileGround(tile);

      if (FindPathsSuccessful()) {
        _playerService.SpendCurrency(_staticDataService.GetStaticData<TileContentStorage>().GetTileConfig(TileContentType.Ground).GoldValue);
        _contentToUpdate.Add(tile.Content);
        return;
      }

      SetTileEmpty(tile);
      FindPathsSuccessful();
    }

    public void RemoveGround(GridTile tile) {
      if (tile.Content.IsGround == false || tile.Content.IsOccupied) {
        return;
      }

      _contentToUpdate.Remove(tile.Content);
      SetTileEmpty(tile);
      FindPathsSuccessful();
      TileContentConfig config = _staticDataService.GetStaticData<TileContentStorage>().GetTileConfig(TileContentType.Ground);
      _playerService.AddCurrency(config.GoldValue);
    }

    public void PlaceTower(GridTile[] tiles, TowerType type, Func<List<Target>> targets) {
      GridTile placementTile = tiles[0];

      foreach (GridTile occupiedTile in tiles) {
        occupiedTile.MakeConjunction(tiles);
      }

      Tower tower = GetTower(type).With(x => x.ReceiveTargets(targets)).With(x => x.HideRadius());
      placementTile.Content.SetOccupiedBy(tower);
      tower.transform.position = GridSystem.GetWorldPosition(placementTile.TilePosition);

      foreach (GridTile occupiedTile in tiles) {
        occupiedTile.Content.SetOccupiedBy(tower);
      }

      _playerService.SpendCurrency(_staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(type).GoldValue);
    }

    public void RemoveTower(GridTile tile) {
      TowerConfig config = _staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(tile.Content.TowerType);
      GridTile[] occupiedTiles = tile.GetOccupied();
      tile.Content.OccupationTower.Recycle();

      foreach (GridTile occupiedTile in occupiedTiles) {
        occupiedTile.Content.ClearOccupation();
        occupiedTile.ClearOccupation();
      }

      _playerService.AddCurrency(config.GoldValue);
    }

    public void PlaceSpawnPoint(GridTile tile, bool isSceneRunPlacement = false) {
      if (tile.NeighborIsSpawnOrDestination) {
        PlaceSpawnPoint(GetRandomEmptyTile());
        return;
      }

      SetTileSpawnPoint(tile);
      _spawnPoints.Add(tile);

      if (isSceneRunPlacement) {
        return;
      }

      TileContentConfig config = _staticDataService.GetStaticData<TileContentStorage>().GetTileConfig(TileContentType.SpawnPoint);
      _playerService.AddCurrency(config.GoldValue);
    }

    public void PlaceEmpty(GridTile tile) => SetTileEmpty(tile);

    public void Clear() {
      foreach (GridTile tile in _gridSystem.GridTileArray) {
        SetTileEmpty(tile);
      }

      ClearLists();
      PlaceDestination(GetRandomEmptyTile(), true);
      PlaceSpawnPoint(GetRandomEmptyTile(), true);
    }

    public GridTile GetRandomEmptyTile() => _gridSystem.GetRandomEmptyTile(_randomService);
    public GridTile GetSpawnPoint(int index) => _spawnPoints[index];
    public GridTile GetDestinationPoints(int index) => _destinationPoints[index];

    public void ClearLists() {
      _contentToUpdate.Clear();
      _searchFrontier.Clear();
      _destinationPoints.Clear();
      _spawnPoints.Clear();
    }

    public void InitialiseGridSystem() => _gridSystem = new GridSystem(_staticDataService.GetStaticData<GridConfig>());

    public void InitialiseGridView() {
      GridConfig gridConfig = _staticDataService.GetStaticData<GridConfig>();
      _gridView = _factory.CreateGameGrid();
      _gridView.InitializeGroundView(gridConfig.GridSize);
      _gridView.InitializeArrowsView(_gridSystem.GridTileArray, gridConfig.CellSize, gridConfig.ArrowSize);
    }

    public GridTile GetTile(Vector3 worldPosition) => _gridSystem.GetGridTile(_gridSystem.GetGridPosition(worldPosition));
    public bool IsValidGridPosition(Vector3 worldPosition) => _gridSystem.IsValidGridPosition(_gridSystem.GetGridPosition(worldPosition));

    private void SetTileEmpty(GridTile tile) => tile.Content = new EmptyContent();

    private void SetTileSpawnPoint(GridTile tile) {
      tile.Content = _factory.Create(TileContentType.SpawnPoint);
      UpdateTileArrowView(tile, true);
    }

    private void SetTileDestination(GridTile tile) {
      tile.Content = _factory.Create(TileContentType.Destination);
      UpdateTileArrowView(tile, true);
    }

    private void SetTileGround(GridTile tile) {
      tile.Content = _factory.Create(TileContentType.Ground);
      UpdateTileArrowView(tile, true);
    }

    private Tower GetTower(TowerType type) => _factory.CreateTower(type);

    private bool FindPathsSuccessful() {
      foreach (GridTile tile in _gridSystem.GridTileArray) {
        if (tile.Content.IsDestination) {
          tile.NullifyDestination();
          _searchFrontier.Enqueue(tile);
        } else {
          tile.ClearPath();
        }
      }

      if (_searchFrontier.Count == 0) {
        return false;
      }

      FrontierSearching();

      foreach (GridTile tile in _gridSystem.GridTileArray) {
        if (tile.HasPath == false) {
          return false;
        }
      }

      UpdatePathView();
      return true;
    }

    private void UpdatePathView() {
      foreach (GridTile tile in _gridSystem.GridTileArray) {
        UpdateTileArrowView(tile);
      }
    }

    private void UpdateTileArrowView(GridTile tile, bool isHidden = false) {
      Vector2[] pathView = tile.GetPathView();

      if (isHidden) {
        pathView = new Vector2[4];
      }

      _gridView.UpdateArrowView(tile.GetUVIndexes(), pathView);
    }

    private void FrontierSearching() {
      while (_searchFrontier.Count > 0) {
        GridTile tile = _searchFrontier.Dequeue();

        if (tile == null) {
          continue;
        }

        if (tile.IsAlternative) {
          _searchFrontier.Enqueue(tile.GrowPathNorth());
          _searchFrontier.Enqueue(tile.GrowPathSouth());
          _searchFrontier.Enqueue(tile.GrowPathEast());
          _searchFrontier.Enqueue(tile.GrowPathWest());
        } else {
          _searchFrontier.Enqueue(tile.GrowPathWest());
          _searchFrontier.Enqueue(tile.GrowPathEast());
          _searchFrontier.Enqueue(tile.GrowPathSouth());
          _searchFrontier.Enqueue(tile.GrowPathNorth());
        }
      }
    }

    public int SpawnPointCount => _spawnPoints.Count;
    public int DestinationPointCount => _destinationPoints.Count;
  }
}