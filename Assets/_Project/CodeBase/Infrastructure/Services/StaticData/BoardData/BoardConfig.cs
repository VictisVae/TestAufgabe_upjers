using CodeBase.BoardContent;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData.BoardData {
  [CreateAssetMenu]
  public class BoardConfig : EntityStaticData {
    public BoardTile BoardTilePrefab;
    public Vector2Int BoardSize;
  }
}