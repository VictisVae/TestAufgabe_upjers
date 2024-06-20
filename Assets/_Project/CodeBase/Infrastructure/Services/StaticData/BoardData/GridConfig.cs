using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData.BoardData {
  [CreateAssetMenu]
  public class GridConfig : EntityStaticData {
    public Vector2Int GridSize;
    public float CellSize;
    [Range(0.1f, 1)]
    public float ArrowSize = 0.4f;
  }
}