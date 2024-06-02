using CodeBase.Infrastructure.Factory;
using UnityEngine;

namespace CodeBase.BoardContent {
  [SelectionBase]
  public class TileContent : FactoryObject {
    [SerializeField]
    private MeshRenderer _meshRenderer;
    [SerializeField]
    private TileContentType _type;
    public Vector2Int Size;
    private IGameFactory _gameFactory;
    private Tower _occupationTower;

    public void ViewAvailable(bool isAvailable) {
      if (_type == TileContentType.Empty) {
        return;
      }
      
      _meshRenderer.material.color = isAvailable ? Color.green : Color.red;
    }

    public void SetNormal() {
      if (_type == TileContentType.Empty) {
        return;
      }

      _meshRenderer.material.color = Color.white;
    }

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

    private void OnDrawGizmosSelected() {
      for (int x = 0; x < Size.x; x++) {
        for (int y = 0; y < Size.y; y++) {
          Gizmos.color = Color.green;
          Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), new Vector3(1,0.01f,1));
        }
      }
    }
  }
}