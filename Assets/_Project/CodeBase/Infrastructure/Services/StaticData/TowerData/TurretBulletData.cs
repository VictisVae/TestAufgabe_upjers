using CodeBase.Infrastructure.Pool;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData.TowerData {
  [CreateAssetMenu]
  public class TurretBulletData : EntityStaticData {
    public TurretBullet Prefab;
    [Range(1, 100)]
    public float Speed;
  }
}