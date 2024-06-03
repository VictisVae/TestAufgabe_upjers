using CodeBase.Units;
using CodeBase.Utilities;
using UnityEngine;
using static CodeBase.Utilities.Constants.Math;

namespace CodeBase.BoardContent {
  public class BoardTile : MonoBehaviour {
    public static void MakeEastWestNeighbour(BoardTile east, BoardTile west) {
      west._east = east;
      east._west = west;
    }

    public static void MakeNorthSouthNeighbour(BoardTile north, BoardTile south) {
      north._south = south;
      south._north = north;
    }

    private static Vector3 GetQuadSide(BoardTile neighbor, Direction direction) => neighbor.transform.localPosition + direction.GetHalfVector();
    private readonly Quaternion _northRotation = Quaternion.Euler(QuarterTurn, 0, 0);
    private readonly Quaternion _eastRotation = Quaternion.Euler(QuarterTurn, QuarterTurn, 0);
    private readonly Quaternion _southRotation = Quaternion.Euler(QuarterTurn, HalfTurn, 0);
    private readonly Quaternion _westRotation = Quaternion.Euler(QuarterTurn, TripleQuarterTurn, 0);
    [SerializeField]
    private Transform _arrow;
    private BoardTile _north, _east, _south, _west;
    private TileContent _content;
    private Conjunction _conjunction;
    private int _distance;

    public void ClearPath() {
      _distance = int.MaxValue;
      NextTileOnOnPath = null;
    }

    public void NullifyDestination() {
      _distance = 0;
      NextTileOnOnPath = null;
      ExitPoint = transform.localPosition;
    }

    public BoardTile GrowPathNorth() => GrowPathTo(_north, Direction.South);
    public BoardTile GrowPathEast() => GrowPathTo(_east, Direction.West);
    public BoardTile GrowPathSouth() => GrowPathTo(_south, Direction.North);
    public BoardTile GrowPathWest() => GrowPathTo(_west, Direction.East);

    public void ShowPath() {
      if (_distance == 0) {
        return;
      }

      _arrow.localRotation = NextTileOnOnPath == _north ? _northRotation :
        NextTileOnOnPath == _east ? _eastRotation :
        NextTileOnOnPath == _south ? _southRotation : _westRotation;
    }

    public void ToggleArrowOn() => _arrow.gameObject.Enable();
    public void ToggleArrowOff() => _arrow.gameObject.Disable();
    public void MakeConjunction(BoardTile[] occupied) => _conjunction = new Conjunction(occupied);
    public BoardTile[] GetOccupied() => _conjunction?.Occupied;
    public void ClearOccupation() => _conjunction = null;

    private BoardTile GrowPathTo(BoardTile neighbor, Direction direction) {
      if (HasPath == false || neighbor == null || neighbor.HasPath) {
        return null;
      }

      neighbor._distance = _distance - 1;
      neighbor.NextTileOnOnPath = this;
      neighbor.ExitPoint = GetQuadSide(neighbor, direction);
      neighbor.PathDirection = direction;
      return neighbor.Content.IsBlockingPath ? null : neighbor;
    }

    public TileContent Content {
      get => _content;
      set {
        if (_content != null) {
          _content.Recycle();
        }

        _content = value;
        _content.transform.localPosition = transform.localPosition;
      }
    }
    public Direction PathDirection { get; private set; }
    public BoardTile NextTileOnOnPath { get; private set; }
    public bool HasPath => _distance != int.MaxValue;
    public Vector3 ExitPoint { get; private set; }
    public bool IsAlternative { get; set; }

    private class Conjunction {
      private readonly BoardTile[] _occupied;
      public Conjunction(BoardTile[] occupied) => _occupied = occupied;
      public BoardTile[] Occupied => _occupied;
    }
  }
}