using CodeBase.Infrastructure.Factory;
using CodeBase.Towers;
using UnityEngine;

namespace CodeBase.BoardContent {
  [SelectionBase]
  public class TileContent : FactoryObject {
    [SerializeField]
    protected TileContentType _type;
    protected IGameFactory _gameFactory;
    public void Construct(IGameFactory gameFactory) => _gameFactory = gameFactory;
    public virtual void Recycle() => _gameFactory.Reclaim(this, _type);
    public virtual void GameUpdate() {}
    public void SetOccupiedBy(Tower tower) => OccupationTower = tower;
    public void ClearOccupation() => OccupationTower = null;
    public virtual void ViewAvailable(bool isAvailable) {}
    public virtual void SetNormal() {}
    public bool IsDestination => _type == TileContentType.Destination;
    public bool IsGround => _type == TileContentType.Ground;
    public bool IsEmpty => _type == TileContentType.Empty;
    public bool IsOccupied => OccupationTower != null;
    public TowerType TowerType => OccupationTower.TowerType;
    public bool IsBlockingPath => _type is TileContentType.Ground;
    public Tower OccupationTower { get; private set; }
  }
}