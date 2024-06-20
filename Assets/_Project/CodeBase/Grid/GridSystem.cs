using CodeBase.Infrastructure.Services.Random;
using CodeBase.Infrastructure.Services.StaticData.BoardData;
using UnityEngine;

namespace CodeBase.Grid {
  public class GridSystem {
    private static Vector2Int _gridSize;
    private static float _cellSize;

    public GridSystem(GridConfig config) {
      _gridSize.x = config.GridSize.x;
      _gridSize.y = config.GridSize.y;
      _cellSize = config.CellSize;
      GridTileArray = new GridTile[_gridSize.x, _gridSize.y];

      for (int x = 0; x < _gridSize.x; x++) {
        for (int y = 0; y < _gridSize.y; y++) {
          GridPosition gridPosition = new GridPosition(new Vector2Int(x, y));
          GridTile newTile = new GridTile(gridPosition);
          GridTileArray[x, y] = newTile;
          SetNeighbours(newTile);
          SetAlternativePath(newTile, x, y);
        }
      }
    }

    private void SetNeighbours(GridTile tile) {
      if (tile.TilePosition.Z > 0) {
        Vector2Int xzPosition = new Vector2Int(tile.TilePosition.X, tile.TilePosition.Z - 1);
        GridTile.MakeNorthSouthNeighbour(tile, GetGridTile(new GridPosition(xzPosition)));
      }

      if (tile.TilePosition.X > 0) {
        Vector2Int xzPosition = new Vector2Int(tile.TilePosition.X - 1, tile.TilePosition.Z);
        GridTile.MakeEastWestNeighbour(tile, GetGridTile(new GridPosition(xzPosition)));
      }
    }

    private void SetAlternativePath(GridTile tile, int x, int y) {
      tile.IsAlternative = (x & 1) == 0;

      if ((y & 1) == 0) {
        tile.IsAlternative = !tile.IsAlternative;
      }
    }

    public GridTile GetGridTile(GridPosition gridPosition) => GridTileArray[gridPosition.X, gridPosition.Z];

    public GridTile GetRandomEmptyTile(IRandomService randomService) {
      int lenghtX = GridTileArray.GetLength(0);
      int lenghtY = GridTileArray.GetLength(1);
      GridTile tile = GridTileArray[randomService.Range(0, lenghtX), randomService.Range(0, lenghtY)];

      while (tile.Content.IsEmpty == false) {
        tile = GridTileArray[randomService.Range(0, lenghtX), randomService.Range(0, lenghtY)];
      }

      return tile;
    }
    
    public bool IsValidGridPosition(GridPosition gridPosition) => gridPosition.X >= 0 && gridPosition.Z >= 0 && gridPosition.X < _gridSize.x && gridPosition.Z < _gridSize.y;

    public GridPosition GetGridPosition(Vector3 worldPosition) =>
      new GridPosition(new Vector2Int(Mathf.RoundToInt(worldPosition.x / _cellSize), Mathf.RoundToInt(worldPosition.z / _cellSize)));

    public static Vector3 GetWorldPosition(GridPosition gridPosition) => new Vector3(gridPosition.X, 0, gridPosition.Z) * _cellSize;
    public GridTile[,] GridTileArray { get; }
  }
}