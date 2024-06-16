using System;
using UnityEngine;

namespace CodeBase.Towers {
  [Serializable]
  public struct TowerBuildScheme {
    [SerializeField]
    private TowerType _towerType;

    public Vector2Int[] GetPlacementScheme() =>
      _towerType switch {
        TowerType.Simple => new[] {
          Vector2Int.zero
        },
        TowerType.Double => new[] {
          new Vector2Int(0, 0),
          new Vector2Int(1, 0),
        },
        TowerType.LTower => new[] {
          new Vector2Int(0, 0),
          new Vector2Int(0, 1),
          new Vector2Int(1, 0)
        },
        TowerType.UTower => new[] {
          new Vector2Int(0, 0),
          new Vector2Int(0, 1),
          new Vector2Int(1, 0),
          new Vector2Int(2, 0),
          new Vector2Int(2, 1),
        },
        _ => new[] { Vector2Int.zero }
      };

    public TowerType TowerType => _towerType;
  }
}