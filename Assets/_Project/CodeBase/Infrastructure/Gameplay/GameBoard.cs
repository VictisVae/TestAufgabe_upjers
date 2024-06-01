using System;
using System.Collections.Generic;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.SceneCreation;
using CodeBase.TowerBehaviour;
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
    private IGameFactory _factory;
    private Vector2Int _boardSize;

    public void Initialize(IStaticDataService staticDataService, IGameFactory contentFactory) {
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
      foreach (TileContent content in _contentToUpdate) {
        content.GameUpdate();
      }
    }

    public void ToggleDestination(BoardTile tile) {
      if (tile.Content.Type == TileContentType.Destination) {
        tile.Content = _factory.Create(TileContentType.Empty);

        if (_destinationPoints.Count > 1) {
          _destinationPoints.Remove(tile);
          tile.Content = _factory.Create(TileContentType.Empty);
        }

        if (FindPathsSuccessful()) {
          return;
        }

        tile.Content = _factory.Create(TileContentType.Destination);
        FindPathsSuccessful();
      } else if (tile.Content.Type == TileContentType.Empty) {
        tile.Content = _factory.Create(TileContentType.Destination);
        FindPathsSuccessful();
      }

      _destinationPoints.Add(tile);
    }

    public void ToggleGround(BoardTile tile) {
      if (tile.Content.Type == TileContentType.Ground) {
        tile.Content = _factory.Create(TileContentType.Empty);
        FindPathsSuccessful();
      } else if (tile.Content.Type == TileContentType.Empty) {
        tile.Content = _factory.Create(TileContentType.Ground);

        if (FindPathsSuccessful()) {
          return;
        }

        tile.Content = _factory.Create(TileContentType.Empty);
        FindPathsSuccessful();
      }
    }

    public void ToggleTower(BoardTile tile, Func<List<TargetPoint>> targets) {
      if (tile.Content.Type == TileContentType.Tower) {
        _contentToUpdate.Remove(tile.Content);
        tile.Content = _factory.Create(TileContentType.Empty);
        FindPathsSuccessful();
      } else if (tile.Content.Type == TileContentType.Empty) {
        tile.Content = _factory.Create(TileContentType.Tower);

        if (FindPathsSuccessful()) {
          _contentToUpdate.Add(tile.Content);
          ((Tower)tile.Content).ReceiveTargets(targets);
          return;
        }

        tile.Content = _factory.Create(TileContentType.Empty);
        FindPathsSuccessful();
      } else if (tile.Content.Type == TileContentType.Ground) {
        tile.Content = _factory.Create(TileContentType.Tower);
        ((Tower)tile.Content).ReceiveTargets(targets);
        _contentToUpdate.Add(tile.Content);
      }
    }

    public void ToggleSpawnPoint(BoardTile tile) {
      if (tile.Content.Type == TileContentType.SpawnPoint) {
        if (_spawnPoints.Count > 1) {
          _spawnPoints.Remove(tile);
          tile.Content = _factory.Create(TileContentType.Empty);
        }
      } else if (tile.Content.Type == TileContentType.Empty) {
        tile.Content = _factory.Create(TileContentType.SpawnPoint);
        _spawnPoints.Add(tile);
      }
    }

    public BoardTile GetTile(Ray ray) {
      if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1) == false) {
        return null;
      }

      int x = (int)(hit.point.x + _boardSize.x * Half);
      int y = (int)(hit.point.z + _boardSize.y * Half);
      bool checkBoarders = x >= 0 && x < _boardSize.x && y >= 0 && y < _boardSize.y;
      return checkBoarders ? CastIntersectionInIndex(x, y) : null;
    }

    public BoardTile GetSpawnPoint(int index) => _spawnPoints[index];
    public BoardTile GetDestinationPoints(int index) => _destinationPoints[index];

    public void Clear() {
      foreach (BoardTile tile in _tiles) {
        tile.Content = _factory.Create(TileContentType.Empty);
      }

      _spawnPoints.Clear();
      _contentToUpdate.Clear();
      ToggleDestination(_tiles[_tiles.Length / 2]);
      ToggleSpawnPoint(_tiles[0]);
    }

    private bool FindPathsSuccessful() {
      foreach (BoardTile tile in _tiles) {
        if (tile.Content.Type == TileContentType.Destination) {
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

    private BoardTile CastIntersectionInIndex(int x, int y) => _tiles[x + y * _boardSize.x];

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