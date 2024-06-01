using UnityEngine;

namespace CodeBase.Infrastructure.Services.AssetManagement {
  public interface IAsset : IService {
    T Initialize<T>(string prefabPath) where T : MonoBehaviour;
    T Initialize<T>(string prefabPath, Vector2 at) where T : MonoBehaviour;
  }
}