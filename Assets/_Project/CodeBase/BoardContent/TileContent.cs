using CodeBase.Infrastructure.Factory;
using CodeBase.Tower;
using UnityEngine;

namespace CodeBase.BoardContent {
  [SelectionBase]
  public class TileContent : FactoryObject {
    [SerializeField]
    protected TileContentType _type;
    protected IGameFactory _gameFactory;
    public void Construct(IGameFactory gameFactory) => _gameFactory = gameFactory;
    public void Recycle() => _gameFactory.Reclaim(this);
    public virtual void GameUpdate() {}
    public void SetOccupiedBy(Tower.Tower tower) => OccupationTower = tower;
    public void ClearOccupation() => OccupationTower = null;
    public virtual void ViewAvailable(bool isAvailable) {}
    public virtual void SetNormal() {}
    public bool IsDestination => _type == TileContentType.Destination;
    public bool IsGround => _type == TileContentType.Ground;
    public bool IsEmpty => _type == TileContentType.Empty;
    public bool IsOccupied => OccupationTower != null;
    public TowerType TowerType => OccupationTower.TowerType;
    public bool IsTower => _type == TileContentType.Tower;
    public bool IsBlockingPath => _type is TileContentType.Ground or TileContentType.Tower;
    public Tower.Tower OccupationTower { get; private set; }
  }
}