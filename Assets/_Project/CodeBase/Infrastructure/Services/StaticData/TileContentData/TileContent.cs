using CodeBase.BoardContent;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData.TileContentData {
  [CreateAssetMenu]
  public class TileContentConfig : ScriptableObject {
    public TileContentType Type;
    public TileContent Prefab;
  }
}