using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.SceneCreation {
  [Serializable]
  public struct FloatRange {
    [SerializeField]
    private float _min, _max;
    public float Min => _min;
    public float Max => _max;
    public float RandomValueInRange => Random.Range(_min, _max);
    public FloatRange(float value) => _min = _max = value;

    public FloatRange(float min, float max) {
      _min = min;
      _max = max;
    }
  }

  public class FloatRangeSliderAttribute : PropertyAttribute {
    public FloatRangeSliderAttribute(float min, float max) {
      Min = min;
      Max = max < min ? min : max;
    }

    public float Min { get; }
    public float Max { get; }
  }
}