using UnityEngine;

namespace CodeBase.Grid {
  public readonly struct GridPosition {
    private readonly Vector2Int _xzPosition;

    public GridPosition(Vector2Int xzPosition) {
      _xzPosition = xzPosition;
    }

    public override string ToString() => $"x: {X}; z: {Z}";
    public int X => _xzPosition.x;
    public int Z => _xzPosition.y;
  }
}