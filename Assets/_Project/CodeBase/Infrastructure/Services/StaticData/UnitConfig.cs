using CodeBase.Data;
using CodeBase.SceneCreation;
using CodeBase.Units;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData {
  [CreateAssetMenu]
  public class UnitConfig : ScriptableObject {
    public UnitType Type;
    public UnitBase Prefab;
    [FloatRangeSlider(0.5f, 2.0f)]
    public FloatRange Scale = new FloatRange(1.0f);
    [FloatRangeSlider(-0.4f, 0.4f)]
    public FloatRange PathOffset = new FloatRange(0.0f);
    [FloatRangeSlider(0.2f, 5.0f)]
    public FloatRange Speed = new FloatRange(1.0f);
  }
}