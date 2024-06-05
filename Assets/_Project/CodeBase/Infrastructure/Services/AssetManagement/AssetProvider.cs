using UnityEngine;

namespace CodeBase.Infrastructure.Services.AssetManagement {
  public class AssetProvider : IAsset {
    public T Initialize<T>(string prefabPath) where T : MonoBehaviour =>
      Object.Instantiate(Resources.Load<T>(prefabPath));

    public T Initialize<T>(string prefabPath, Vector2 at) where T : MonoBehaviour =>
      Object.Instantiate(Resources.Load<T>(prefabPath), at, Quaternion.identity);
  }
}