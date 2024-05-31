using System;
using CodeBase.Data;
using CodeBase.SceneCreation;
using CodeBase.Units;
using UnityEngine;

namespace CodeBase.Factory {
  [CreateAssetMenu]
  public class UnitFactory : BaseFactory {
    [SerializeField]
    private UnitConfig _small, _medium, _large, _air;
    public void Reclaim(UnitBase unit) => Destroy(unit.gameObject);

    public UnitBase Get(UnitType type) {
      var config = GetConfig(type);
      UnitBase instance = CreateGameObjectInstance(config._prefab);
      instance.Initialise(this, config.Scale.RandomValueInRange, config.PathOffset.RandomValueInRange, config.Speed.RandomValueInRange);
      return instance;
    }

    private UnitConfig GetConfig(UnitType type) =>
      type switch {
        UnitType.Large => _large,
        UnitType.Medium => _medium,
        UnitType.Small => _small,
        UnitType.Air => _air,
        _ => _medium
      };

    [Serializable]
    private class UnitConfig {
      [FloatRangeSlider(0.5f, 2.0f)]
      public FloatRange Scale = new FloatRange(1.0f);
      [FloatRangeSlider(-0.4f, 0.4f)]
      public FloatRange PathOffset = new FloatRange(0.0f);
      [FloatRangeSlider(0.2f, 5.0f)]
      public FloatRange Speed = new FloatRange(1.0f);
      public UnitBase _prefab;
    }
  }
}