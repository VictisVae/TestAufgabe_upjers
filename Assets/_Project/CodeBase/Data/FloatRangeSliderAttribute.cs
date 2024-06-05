using UnityEngine;

namespace CodeBase.Data {
  public class FloatRangeSliderAttribute : PropertyAttribute {
    public FloatRangeSliderAttribute(float min, float max) {
      Min = min;
      Max = max < min ? min : max;
    }

    public float Min { get; }
    public float Max { get; }
  }
}