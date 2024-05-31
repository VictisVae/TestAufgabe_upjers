using CodeBase.Units;
using UnityEngine;
using static CodeBase.Extensions.Constants.Math;

namespace CodeBase.Extensions {
  public static class Extensions {
    public static void Activate(this GameObject gameObject) => gameObject.SetActive(true);
    public static void Deactivate(this GameObject gameObject) => gameObject.SetActive(false);
  }

  public static class DirectionExtensions {
    private static readonly Quaternion[] Rotations = {
      Quaternion.identity,
      Quaternion.Euler(0, QuarterTurn, 0),
      Quaternion.Euler(0, HalfTurn, 0),
      Quaternion.Euler(0, TripleQuarterTurn, 0)
    };
    private static readonly Vector3[] HalfVectors = {
      Vector3.forward * Half,
      Vector3.right * Half,
      Vector3.back * Half,
      Vector3.left * Half
    };
    public static Quaternion GetRotation(this Direction direction) => Rotations[(int)direction];

    public static DirectionChange GetDirectionChangeTo(this Direction current, Direction next) {
      if (current == next) {
        return DirectionChange.None;
      }

      if (current + 1 == next || current - 3 == next) {
        return DirectionChange.TurnRight;
      }

      if (current - 1 == next || current + 3 == next) {
        return DirectionChange.TurnLeft;
      }

      return DirectionChange.TurnAround;
    }

    public static float GetAngle(this Direction direction) => (float)direction * QuarterTurn;
    public static Vector3 GetHalfVector(this Direction direction) => HalfVectors[(int)direction];
  }
}