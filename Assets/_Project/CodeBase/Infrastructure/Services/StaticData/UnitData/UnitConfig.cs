using CodeBase.Data;
using CodeBase.Units;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData.UnitData {
  [CreateAssetMenu]
  public class UnitConfig : ScriptableObject {
    public UnitType Type;
    public UnitBase Prefab;
    [Range(10, 500)]
    public int Health;
    public int Gold = 10;
    [FloatRangeSlider(0.1f, 2.0f)]
    public FloatRange Scale = new FloatRange(1.0f);
    [FloatRangeSlider(-0.4f, 0.4f)]
    public FloatRange PathOffset = new FloatRange(0.0f);
    [FloatRangeSlider(0.2f, 5.0f)]
    public FloatRange Speed = new FloatRange(1.0f);
  }
}