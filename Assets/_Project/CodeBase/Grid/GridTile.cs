using CodeBase.BoardContent;
using CodeBase.Units;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Grid {
  public class GridTile {
    public static void MakeEastWestNeighbour(GridTile east, GridTile west) {
      west._east = east;
      east._west = west;
    }

    public static void MakeNorthSouthNeighbour(GridTile north, GridTile south) {
      north._south = south;
      south._north = north;
    }

    private static Vector3 GetTileSide
      (GridTile neighbor, Direction direction) => GridSystem.GetWorldPosition(neighbor._tilePosition) + direction.GetHalfVector();

    private readonly GridPosition _tilePosition;
    private GridUVData _gridUVData;
    private GridTile _north, _east, _south, _west;
    private ITileContent _content;
    private Conjunction _conjunction;
    private int _distance;
    public GridTile(GridPosition tilePosition) => _tilePosition = tilePosition;
    public override string ToString() => _tilePosition.ToString();

    public void ClearPath() {
      _distance = int.MaxValue;
      NextTileOnOnPath = null;
    }

    public void NullifyDestination() {
      _distance = 0;
      NextTileOnOnPath = null;
      ExitPoint = GridSystem.GetWorldPosition(_tilePosition);
    }

    public GridTile GrowPathNorth() => GrowPathTo(_north, Direction.South);
    public GridTile GrowPathEast() => GrowPathTo(_east, Direction.West);
    public GridTile GrowPathSouth() => GrowPathTo(_south, Direction.North);
    public GridTile GrowPathWest() => GrowPathTo(_west, Direction.East);

    public Vector2[] GetPathView() {
      if (_distance == 0 || _content.IsEmpty == false) {
        return new Vector2[4];
      }

      return NextTileOnOnPath == _north ? GridUVData.GetUVDirectionUp() :
        NextTileOnOnPath == _east ? GridUVData.GetUVDirectionRight() :
        NextTileOnOnPath == _south ? GridUVData.GetUVDirectionDown() : GridUVData.GetUVDirectionLeft();
    }

    public void MakeConjunction(GridTile[] occupied) => _conjunction = new Conjunction(occupied);
    public GridTile[] GetOccupied() => _conjunction?.Occupied;
    public void ClearOccupation() => _conjunction = null;
    public void SetUVData(GridUVData gridUVData) => _gridUVData = gridUVData;
    public int[] GetUVIndexes() => _gridUVData.GetUVIndexes;

    private GridTile GrowPathTo(GridTile neighbor, Direction direction) {
      if (HasPath == false || neighbor == null || neighbor.HasPath) {
        return null;
      }

      neighbor._distance = _distance - 1;
      neighbor.NextTileOnOnPath = this;
      neighbor.ExitPoint = GetTileSide(neighbor, direction);
      neighbor.PathDirection = direction;
      return neighbor.Content.IsGround ? null : neighbor;
    }

    public ITileContent Content {
      get => _content;
      set {
        if (_content != null) {
          _content.Recycle();
        }

        _content = value;
        _content.SetPosition(GridSystem.GetWorldPosition(_tilePosition));
      }
    }
    public Direction PathDirection { get; private set; }
    public GridTile NextTileOnOnPath { get; private set; }
    public bool HasPath => _distance != int.MaxValue;
    public bool NeighborIsSpawnOrDestination {
      get {
        GridTile[] neighbors = { _north, _east, _south, _west };

        foreach (GridTile neighbor in neighbors) {
          if (neighbor is null) {
            continue;
          }

          if (neighbor.Content.IsDestination || neighbor.Content.IsSpawnPoint) {
            return true;
          }
        }

        return false;
      }
    }
    public bool IsAlternative { get; set; }
    public Vector3 ExitPoint { get; private set; }
    public Vector3 WorldPosition => GridSystem.GetWorldPosition(TilePosition);
    public GridPosition TilePosition => _tilePosition;

    private class Conjunction {
      private readonly GridTile[] _occupied;
      public Conjunction(GridTile[] occupied) => _occupied = occupied;
      public GridTile[] Occupied => _occupied;
    }
  }
}