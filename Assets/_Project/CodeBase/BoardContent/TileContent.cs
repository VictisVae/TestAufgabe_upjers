using CodeBase.Infrastructure.Factory;
using UnityEngine;

namespace CodeBase.BoardContent {
  [SelectionBase]
  public class TileContent : FactoryObject {
    [SerializeField]
    private TileContentType _type;
    private IGameFactory _gameFactory;
    private Tower _occupationTower;
    public void Construct(IGameFactory gameFactory) => _gameFactory = gameFactory;
    public void Recycle() => _gameFactory.Reclaim(this);
    public virtual void GameUpdate() {}
    public void SetOccupiedBy(Tower tower) => _occupationTower = tower;
    public void ClearOccupation() => _occupationTower = null;
    public bool IsSpawnPoint => _type == TileContentType.SpawnPoint;
    public bool IsDestination => _type == TileContentType.Destination;
    public bool IsGround => _type == TileContentType.Ground;
    public bool IsEmpty => _type == TileContentType.Empty;
    public bool IsOccupied => _occupationTower is not null;
    public bool IsBlockingPath => _type is TileContentType.Ground or TileContentType.Tower;
  }
}