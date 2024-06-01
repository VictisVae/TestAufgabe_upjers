using CodeBase.SceneCreation;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData {
  [CreateAssetMenu]
  public class TileContentConfig : ScriptableObject {
    public TileContentType Type;
    public TileContent Prefab;
  }
}