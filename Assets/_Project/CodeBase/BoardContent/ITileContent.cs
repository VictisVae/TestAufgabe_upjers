using CodeBase.Towers;
using UnityEngine;

namespace CodeBase.BoardContent {
  public interface ITileContent {
    public void SetPosition(Vector3 position);
    public void Recycle() {}
    public bool IsDestination { get; }
    public bool IsSpawnPoint { get; }
    public bool IsGround { get; }
    public bool IsEmpty { get; }
    public bool IsOccupied { get; }
    public void SetOccupiedBy(ITower tower) {}
    public void ClearOccupation() {}
    public TowerType TowerType { get; }
    public ITower OccupationTower { get; }
    public void GameUpdate() {}
  }
}