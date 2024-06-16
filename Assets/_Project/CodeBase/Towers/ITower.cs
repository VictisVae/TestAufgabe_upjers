namespace CodeBase.Towers {
  public interface ITower {
    void GameUpdate();
    TowerType TowerType { get; }
    void Recycle();
  }
}