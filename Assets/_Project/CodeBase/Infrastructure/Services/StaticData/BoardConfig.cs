using CodeBase.SceneCreation;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData {
  [CreateAssetMenu]
  public class BoardConfig : EntityStaticData {
    public BoardTile BoardTilePrefab;
    public Vector2Int BoardSize;
  }
}