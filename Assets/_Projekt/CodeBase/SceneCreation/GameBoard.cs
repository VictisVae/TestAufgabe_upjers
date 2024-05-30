using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.SceneCreation {
  public class GameBoard : MonoBehaviour {
    [SerializeField]
    private Transform _ground;
    [SerializeField]
    private BoardTile _tilePrefab;
    private Vector2Int _boardSize;
    private BoardTile[] _tiles;
    private readonly Queue<BoardTile> _searchFrontier = new Queue<BoardTile>();

    public void Initialize(Vector2Int size) {
      _boardSize = size;
      _ground.localScale = new Vector3(size.x, size.y, 1.0f);
      Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
      _tiles = new BoardTile[size.x * size.y];

      for (int i = 0, y = 0; y < size.y; y++) {
        for (int x = 0; x < size.x; x++, i++) {
          BoardTile tile = CreateTile(i, x, offset, y);
          SetNeighbours(size, y, tile, i, x);
          SetAlternativePath(tile, x, y);
        }
      }

      FindPaths();
    }

    public void FindPaths() {
      foreach (BoardTile tile in _tiles) {
        tile.ClearPath();
      }

      int destinationIndex = _tiles.Length / 2;
      _tiles[destinationIndex].ReceiveDestination();
      _searchFrontier.Enqueue(_tiles[destinationIndex]);
      FrontierSearching();

      foreach (BoardTile tile in _tiles) {
        tile.ShowPath();
      }
    }

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
  }
}