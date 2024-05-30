using CodeBase.Extensions;
using UnityEngine;

namespace CodeBase.SceneCreation {
  public class BoardTile : MonoBehaviour {
    public static void MakeEastWestNeighbour(BoardTile east, BoardTile west) {
      west._east = east;
      east._west = west;
    }

    public static void MakeNorthSouthNeighbour(BoardTile north, BoardTile south) {
      north._south = south;
      south._north = north;
    }

    private readonly Quaternion _northRotation = Quaternion.Euler(90f, 0, 0);
    private readonly Quaternion _eastRotation = Quaternion.Euler(90f, 90, 0);
    private readonly Quaternion _southRotation = Quaternion.Euler(90f, 180, 0);
    private readonly Quaternion _westRotation = Quaternion.Euler(90f, 270, 0);
    [SerializeField]
    private Transform _arrow;
    private BoardTile _north, _east, _south, _west, _nextPathTile;
    private int _distance;
    private TileContent _content;

    public void ClearPath() {
      _distance = int.MaxValue;
      _nextPathTile = null;
    }

    public void ReceiveDestination() {
      _distance = 0;
      _nextPathTile = null;
    }

    public BoardTile GrowPathNorth() => GrowPathTo(_north);
    public BoardTile GrowPathEast() => GrowPathTo(_east);
    public BoardTile GrowPathSouth() => GrowPathTo(_south);
    public BoardTile GrowPathWest() => GrowPathTo(_west);

    public void ShowPath() {
      if (_distance == 0) {
        _arrow.gameObject.Deactivate();
        return;
      }

      _arrow.gameObject.Activate();

      _arrow.localRotation = _nextPathTile == _north ? _northRotation :
        _nextPathTile == _east ? _eastRotation :
        _nextPathTile == _south ? _southRotation : _westRotation;
    }

    private BoardTile GrowPathTo(BoardTile neighbor) {
      if (HasPath == false || neighbor == null || neighbor.HasPath) {
        return null;
      }

      neighbor._distance = _distance - 1;
      neighbor._nextPathTile = this;
      return neighbor.Content.Type != TileContentType.Ground ? neighbor : null;
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
    
    public bool HasPath => _distance != int.MaxValue;
    public bool IsAlternative { get; set; }
  }
}