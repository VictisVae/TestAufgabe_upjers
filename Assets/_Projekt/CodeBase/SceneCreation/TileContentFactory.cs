using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.SceneCreation {
  [CreateAssetMenu]
  public class TileContentFactory : ScriptableObject {
    [SerializeField]
    private TileContent _destinationPrefab;
    [SerializeField]
    private TileContent _emptyPrefab;
    [SerializeField]
    private TileContent _groundPrefab;
    
    private Scene _contentScene;
    public void Reclaim(TileContent content) => Destroy(content.gameObject);

    public TileContent Get(TileContentType type) =>
      type switch {
        TileContentType.Empty => Get(_emptyPrefab),
        TileContentType.Destination => Get(_destinationPrefab),
        TileContentType.Ground => Get(_groundPrefab),
        _ => null
      };

    private TileContent Get(TileContent prefab) {
      TileContent instance = Instantiate(prefab);
      instance.TileContentFactory = this;
      MoveToFactoryScene(instance.gameObject);
      return instance;
    }

    private void MoveToFactoryScene(GameObject go) {
      if (_contentScene.isLoaded == false) {
        if (Application.isEditor) {
          _contentScene = SceneManager.GetSceneByName(name);

          if (_contentScene.isLoaded == false) {
            _contentScene = SceneManager.CreateScene(name);
          }
        } else {
          _contentScene = SceneManager.CreateScene(name);
        }
      }

      SceneManager.MoveGameObjectToScene(go, _contentScene);
    }
  }
}