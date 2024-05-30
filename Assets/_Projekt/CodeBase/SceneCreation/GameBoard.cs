using System.Collections.Generic;
using CodeBase.Factory;
using UnityEngine;

namespace CodeBase.SceneCreation {
  public class GameBoard : MonoBehaviour {
    private readonly Queue<BoardTile> _searchFrontier = new Queue<BoardTile>();
    [SerializeField]
    private Transform _ground;
    [SerializeField]
    private BoardTile _tilePrefab;
    private Vector2Int _boardSize;
    private BoardTile[] _tiles;
    private TileContentFactory _contentFactory;
    private readonly List<BoardTile> _spawnPoints = new List<BoardTile>();

    public void Initialize(Vector2Int size, TileContentFactory contentFactory) {
      _boardSize = size;
      _ground.localScale = new Vector3(size.x, size.y, 1.0f);
      Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
      _tiles = new BoardTile[size.x * size.y];
      _contentFactory = contentFactory;

      for (int i = 0, y = 0; y < size.y; y++) {
        for (int x = 0; x < size.x; x++, i++) {
          BoardTile tile = CreateTile(i, x, offset, y);
          SetNeighbours(size, y, tile, i, x);
          SetAlternativePath(tile, x, y);
          tile.Content = _contentFactory.Get(TileContentType.Empty);
        }
      }

      ToggleDestination(_tiles[_tiles.Length / 2]);
      ToggleSpawnPoint(_tiles[0]);
    }

    public bool FindPathsSuccessful() {
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

    public void ToggleDestination(BoardTile tile) {
      if (tile.Content.Type == TileContentType.Destination) {
        tile.Content = _contentFactory.Get(TileContentType.Empty);

        if (FindPathsSuccessful()) {
          return;
        }

        tile.Content = _contentFactory.Get(TileContentType.Destination);
        FindPathsSuccessful();
      } else if (tile.Content.Type == TileContentType.Empty) {
        tile.Content = _contentFactory.Get(TileContentType.Destination);
        FindPathsSuccessful();
      }
    }

    public void ToggleGround(BoardTile tile) {
      if (tile.Content.Type == TileContentType.Ground) {
        tile.Content = _contentFactory.Get(TileContentType.Empty);
        FindPathsSuccessful();
      } else if (tile.Content.Type == TileContentType.Empty) {
        tile.Content = _contentFactory.Get(TileContentType.Ground);

        if (FindPathsSuccessful()) {
          return;
        }

        tile.Content = _contentFactory.Get(TileContentType.Empty);
        FindPathsSuccessful();
      }
    }

    public void ToggleSpawnPoint(BoardTile tile) {
      if (tile.Content.Type == TileContentType.SpawnPoint) {
        if (_spawnPoints.Count > 1) {
          _spawnPoints.Remove(tile);
          tile.Content = _contentFactory.Get(TileContentType.Empty);
        }
      } else if (tile.Content.Type == TileContentType.Empty) {
        tile.Content = _contentFactory.Get(TileContentType.SpawnPoint);
        _spawnPoints.Add(tile);
      }
    }

    public BoardTile GetTile(Ray ray) {
      if (Physics.Raycast(ray, out RaycastHit hit) == false) {
        return null;
      }

      int x = (int)(hit.point.x + _boardSize.x * 0.5f);
      int y = (int)(hit.point.z + _boardSize.y * 0.5f);
      bool checkBoarders = x >= 0 && x < _boardSize.x && y >= 0 && y < _boardSize.y;
      return checkBoarders ? CastIntersectionInIndex(x, y) : null;
    }

    public BoardTile GetSpawnPoint(int index) => _spawnPoints[index];
    private BoardTile CastIntersectionInIndex(int x, int y) => _tiles[x + y * _boardSize.x];

    private BoardTile CreateTile(int i, int x, Vector2 offset, int y) {
      BoardTile tile = _tiles[i] = Instantiate(_tilePrefab, transform, false);
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
  }
}