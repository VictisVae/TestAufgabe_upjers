using CodeBase.SceneCreation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Factory {
  [CreateAssetMenu]
  public class TileContentFactory : BaseFactory {
    [SerializeField]
    private TileContent _destinationPrefab;
    [SerializeField]
    private TileContent _emptyPrefab;
    [SerializeField]
    private TileContent _groundPrefab;
    [SerializeField]
    private TileContent _spawnPointPrefab;

    private Scene _contentScene;
    public void Reclaim(TileContent content) => Destroy(content.gameObject);

    public TileContent Get(TileContentType type) =>
      type switch {
        TileContentType.Empty => Get(_emptyPrefab),
        TileContentType.Destination => Get(_destinationPrefab),
        TileContentType.Ground => Get(_groundPrefab),
        TileContentType.SpawnPoint => Get(_spawnPointPrefab),
        _ => null
      };

    private TileContent Get(TileContent prefab) {
      TileContent instance = CreateGameObjectInstance(prefab);
      instance.TileContentFactory = this;
      return instance;
    }
  }
}