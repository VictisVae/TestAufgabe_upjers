using CodeBase.Towers;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData.TowerData {
  [CreateAssetMenu]
  public class TowerConfig : ScriptableObject {
    public TowerBuildScheme BuildScheme;
    [Range(0.1f, 10.0f)]
    public float ShootFrequency;
    [Range(1, 50)]
    public int LaserDamage;
    public int GoldValue;
    public Tower Prefab;
  }
}