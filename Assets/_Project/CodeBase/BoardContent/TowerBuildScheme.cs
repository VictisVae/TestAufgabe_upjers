using System;
using UnityEngine;

namespace CodeBase.BoardContent {
  [Serializable]
  public struct TowerBuildScheme {
    [SerializeField]
    private TowerType _towerType;
    public TowerType TowerType => _towerType;

    public Vector2Int[] GetPlacementScheme() =>
      _towerType switch {
        TowerType.Simple => new[] {
          Vector2Int.zero
        },
        TowerType.Double => new[] {
          new Vector2Int(0, 0),
          new Vector2Int(1, 0)
        },
        TowerType.Quad => new[] {
          new Vector2Int(0, 0),
          new Vector2Int(0, 1),
          new Vector2Int(1, 0),
          new Vector2Int(1, 1)
        },
        TowerType.LTower => new[] {
          new Vector2Int(0, 0),
          new Vector2Int(0, 1),
          new Vector2Int(0, 2),
          new Vector2Int(1, 0)
        },
        TowerType.UTower => new[] {
          new Vector2Int(0, 0),
          new Vector2Int(0, 1),
          new Vector2Int(0, 2),
          new Vector2Int(1, 0),
          new Vector2Int(2, 0),
          new Vector2Int(2, 1),
          new Vector2Int(2, 2)
        },
        TowerType.PlusTower => new[] {
          new Vector2Int(0, 1),
          new Vector2Int(1, 0),
          new Vector2Int(1, 1),
          new Vector2Int(1, 2),
          new Vector2Int(2, 1)
        },
        _ => new[] { Vector2Int.zero }
      };
  }
}