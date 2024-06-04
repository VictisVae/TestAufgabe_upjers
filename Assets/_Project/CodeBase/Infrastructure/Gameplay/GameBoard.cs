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
using CodeBase.TowerBehaviour;
using CodeBase.Utilities;
using UnityEngine;
using static CodeBase.Utilities.Constants.Math;

namespace CodeBase.Infrastructure.Gameplay {
  public class GameBoard : MonoBehaviour {
    private readonly Queue<BoardTile> _searchFrontier = new Queue<BoardTile>();
    private readonly List<BoardTile> _spawnPoints = new List<BoardTile>();
    private readonly List<BoardTile> _destinationPoints = new List<BoardTile>();
    private readonly List<TileContent> _contentToUpdate = new List<TileContent>();
    [SerializeField]
    private Transform _ground;
    private BoardTile[] _tiles;
    private IStaticDataService _staticDataService;
    private IRandomService _randomService;
    private IGameFactory _factory;
    private IPlayerService _playerService;
    private Vector2Int _boardSize;

    public void Initialize
      (IStaticDataService staticDataService, IRandomService randomService, IGameFactory contentFactory, IPlayerService playerService) {
      _playerService = playerService;
      _randomService = randomService;
      _factory = contentFactory;
      _staticDataService = staticDataService;
      _boardSize = staticDataService.GetStaticData<BoardConfig>().BoardSize;
      _ground.localScale = new Vector3(_boardSize.x, _boardSize.y, 1.0f);
      Vector2 offset = new Vector2((_boardSize.x - 1) * Half, (_boardSize.y - 1) * Half);
      _tiles = new BoardTile[_boardSize.x * _boardSize.y];

      for (int i = 0, y = 0; y < _boardSize.y; y++) {
        for (int x = 0; x < _boardSize.x; x++, i++) {
          BoardTile tile = CreateTile(i, x, offset, y);
          SetNeighbours(_boardSize, y, tile, i, x);
          SetAlternativePath(tile, x, y);
        }
      }

      Clear();
    }

    public void GameUpdate() {
      foreach (TileContent tileContent in _contentToUpdate) {
        tileContent.GameUpdate();
      }
    }

    public void PlaceDestination(BoardTile tile, bool isSceneRunPlacement = false) {
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

    public void PlaceGround(BoardTile tile) {
      SetTileGround(tile);

      if (FindPathsSuccessful()) {
        _playerService.SpendCurrency(_staticDataService.GetStaticData<TileContentStorage>().GetTileConfig(TileContentType.Ground).GoldValue);
        return;
      }

      SetTileEmpty(tile);
      FindPathsSuccessful();
    }

    public void RemoveGround(BoardTile tile) {
      if (tile.Content.IsGround == false || tile.Content.IsOccupied) {
        return;
      }

      SetTileEmpty(tile);
      FindPathsSuccessful();
      TileContentConfig config = _staticDataService.GetStaticData<TileContentStorage>().GetTileConfig(TileContentType.Ground);
      _playerService.AddCurrency(config.GoldValue);
    }

    public void PlaceTower(BoardTile[] tiles, TowerType type, Func<List<Target>> targets) {
      BoardTile placementTile = tiles[0];

      foreach (BoardTile occupiedTile in tiles) {
        occupiedTile.MakeConjunction(tiles);
      }

      Tower tower = GetTower(type).With(x => x.ReceiveTargets(targets));
      placementTile.Content.SetOccupiedBy(tower);
      tower.transform.position = placementTile.transform.position;

      foreach (BoardTile occupiedTile in tiles) {
        occupiedTile.Content.SetOccupiedBy(tower);
      }

      _playerService.SpendCurrency(_staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(type).GoldValue);
      _contentToUpdate.Add(tower);
    }

    public void RemoveTower(BoardTile tile) {
      TowerConfig config = _staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(tile.Content.TowerType);
      BoardTile[] occupiedTiles = tile.GetOccupied();
      _contentToUpdate.Remove(tile.Content.OccupationTower);
      tile.Content.OccupationTower.Recycle();

      foreach (BoardTile occupiedTile in occupiedTiles) {
        SetTileGround(occupiedTile);
        occupiedTile.Content.ClearOccupation();
        occupiedTile.ClearOccupation();
      }

      _playerService.AddCurrency(config.GoldValue);
    }

    public void PlaceSpawnPoint(BoardTile tile, bool isSceneRunPlacement = false) {
      SetTileSpawnPoint(tile);
      _spawnPoints.Add(tile);

      if (isSceneRunPlacement) {
        return;
      }

      TileContentConfig config = _staticDataService.GetStaticData<TileContentStorage>().GetTileConfig(TileContentType.SpawnPoint);
      _playerService.AddCurrency(config.GoldValue);
    }

    public void PlaceEmpty(BoardTile tile) => SetTileEmpty(tile);
    public BoardTile GetTile(float posX, float posZ) => IsInBorders(posX, posZ, out int x, out int y) ? CastIntersectionIndex(x, y) : null;

    public BoardTile GetRandomEmptyTile() {
      BoardTile tile = _tiles[_randomService.Range(0, _tiles.Length)];

      while (tile.Content.IsEmpty == false) {
        tile = _tiles[_randomService.Range(0, _tiles.Length)];
      }

      return tile;
    }

    public BoardTile GetSpawnPoint(int index) => _spawnPoints[index];
    public BoardTile GetDestinationPoints(int index) => _destinationPoints[index];

    private bool IsInBorders(float posX, float posZ, out int x, out int y) {
      x = (int)(posX + _boardSize.x * Half);
      y = (int)(posZ + _boardSize.y * Half);
      return x >= 0 && x < _boardSize.x && y >= 0 && y < _boardSize.y;
    }

    public void Clear() {
      foreach (BoardTile tile in _tiles) {
        SetTileEmpty(tile);
      }

      _searchFrontier.Clear();
      _destinationPoints.Clear();
      _spawnPoints.Clear();
      _contentToUpdate.Clear();
      PlaceDestination(GetRandomEmptyTile(), true);
      PlaceSpawnPoint(GetRandomEmptyTile(), true);
    }

    private void SetTileEmpty(BoardTile tile) {
      tile.Content = _factory.Create(TileContentType.Empty);
      tile.ToggleArrowOn();
    }

    private void SetTileSpawnPoint(BoardTile tile) {
      tile.Content = _factory.Create(TileContentType.SpawnPoint);
      tile.ToggleArrowOff();
    }

    private void SetTileDestination(BoardTile tile) {
      tile.Content = _factory.Create(TileContentType.Destination);
      tile.ToggleArrowOff();
    }

    private void SetTileGround(BoardTile tile) {
      tile.Content = _factory.Create(TileContentType.Ground);
      tile.ToggleArrowOff();
    }

    private Tower GetTower(TowerType type) => _factory.CreateTower(type);

    private bool FindPathsSuccessful() {
      foreach (BoardTile tile in _tiles) {
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

      foreach (BoardTile tile in _tiles) {
        if (tile.HasPath == false) {
          return false;
        }
      }

      foreach (BoardTile tile in _tiles) {
        tile.ShowPath();
      }

      return true;
    }

    private BoardTile CastIntersectionIndex(int x, int y) => _tiles[x + y * _boardSize.x];

    private BoardTile CreateTile(int i, int x, Vector2 offset, int y) {
      BoardTile tile = _tiles[i] = Instantiate(_staticDataService.GetStaticData<BoardConfig>().BoardTilePrefab, transform, false);
      tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
      return tile;
    }

    private void SetNeighbours(Vector2Int size, int y, BoardTile tile, int i, int x) {
      if (y > 0) {
        BoardTile.MakeNorthSouthNeighbour(tile, _tiles[i - size.x]);
      }

      if (x > 0) {
        BoardTile.MakeEastWestNeighbour(tile, _tiles[i - 1]);
      }
    }

    private void SetAlternativePath(BoardTile tile, int x, int y) {
      tile.IsAlternative = (x & 1) == 0;

      if ((y & 1) == 0) {
        tile.IsAlternative = !tile.IsAlternative;
      }
    }

    private void FrontierSearching() {
      while (_searchFrontier.Count > 0) {
        BoardTile tile = _searchFrontier.Dequeue();

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