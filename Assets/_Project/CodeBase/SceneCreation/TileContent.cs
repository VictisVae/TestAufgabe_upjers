using CodeBase.Infrastructure.Factory;
using UnityEngine;

namespace CodeBase.SceneCreation {
  [SelectionBase]
  public class TileContent : FactoryObject {
    [SerializeField]
    private TileContentType _type;
    private IGameFactory _gameFactory;
    public void Construct(IGameFactory gameFactory) => _gameFactory = gameFactory;
    public void Recycle() => _gameFactory.Reclaim(this);
    public virtual void GameUpdate() {}
    public TileContentType Type => _type;
    public bool IsBlockingPath => Type is TileContentType.Ground or TileContentType.Tower;
  }
}