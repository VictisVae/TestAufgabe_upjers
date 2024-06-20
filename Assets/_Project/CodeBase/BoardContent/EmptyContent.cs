using CodeBase.Towers;
using UnityEngine;

namespace CodeBase.BoardContent {
  public class EmptyContent : ITileContent {
    public void SetPosition(Vector3 position) {}
    public bool IsDestination => false;
    public bool IsSpawnPoint => false;
    public bool IsGround => false;
    public bool IsEmpty => true;
    public bool IsOccupied => false;
    public TowerType TowerType => TowerType.Null;
    public ITower OccupationTower => new NullTower();
  }
}