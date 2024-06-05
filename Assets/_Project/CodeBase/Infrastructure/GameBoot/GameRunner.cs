using UnityEngine;

namespace CodeBase.Infrastructure.GameBoot {
  public class GameRunner : MonoBehaviour {
    [SerializeField]
    private GameBootstrapper _bootstrapperPrefab;

    private void Awake() {
      var bootstrapper = FindObjectOfType<GameBootstrapper>();

      if (bootstrapper != null) {
        Destroy(gameObject);
        return;
      }

      Instantiate(_bootstrapperPrefab);
    }
  }
}