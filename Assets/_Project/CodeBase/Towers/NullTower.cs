namespace CodeBase.Towers {
  public class NullTower : ITower {
    public void GameUpdate() {}
    public TowerType TowerType { get; set; } = TowerType.Null;
    public void Recycle() {}
  }
}