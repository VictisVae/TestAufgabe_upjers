using CodeBase.BoardContent;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData.TowerData {
  [CreateAssetMenu]
  public class TowerConfig : ScriptableObject {
    public TileContentType ContentType = TileContentType.Tower;
    public TowerBuildScheme BuildScheme;
    public Tower Prefab;
  }
}