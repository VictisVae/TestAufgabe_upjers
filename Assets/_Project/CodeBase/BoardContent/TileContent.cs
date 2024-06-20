using CodeBase.Infrastructure.Factory;
using CodeBase.Towers;
using UnityEngine;

namespace CodeBase.BoardContent {
  [SelectionBase]
  public class TileContent : FactoryObject, ITileContent {
    [SerializeField]
    protected TileContentType _type;
    private IGameFactory _gameFactory;

    public void Construct(IGameFactory gameFactory) {
      _gameFactory = gameFactory;
      ClearOccupation();
    }

    public void SetPosition(Vector3 position) => transform.localPosition = position;
    public override void Recycle() => _gameFactory.Reclaim(this, _type);

    public void GameUpdate() {
      if (_type != TileContentType.Ground) {
        return;
      }

      OccupationTower.GameUpdate();
    }

    public void SetOccupiedBy(ITower tower) => OccupationTower = tower;
    public void ClearOccupation() => OccupationTower = new NullTower();
    public bool IsDestination => _type == TileContentType.Destination;
    public bool IsSpawnPoint => _type == TileContentType.SpawnPoint;
    public bool IsGround => _type == TileContentType.Ground;
    public bool IsEmpty => _type == TileContentType.Empty;
    public bool IsOccupied => OccupationTower is not NullTower;
    public TowerType TowerType => OccupationTower.TowerType;
    public ITower OccupationTower { get; private set; }
  }
}