using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData.Player {
  [CreateAssetMenu]
  public class PlayerStaticData : EntityStaticData {
    [Range(1, 100)]
    public int Health;
    public int Gold;
  }
}