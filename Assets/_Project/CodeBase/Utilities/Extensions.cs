using System;
using CodeBase.Units;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static CodeBase.Utilities.Constants.Math;

namespace CodeBase.Utilities {
  public static class Extensions {
    public static void Enable(this GameObject self) => self.SetActive(true);
    public static void Disable(this GameObject self) => self.SetActive(false);
    public static void Enable(this Transform self) => self.gameObject.SetActive(true);
    public static void Disable(this Transform self) => self.gameObject.SetActive(false);
    
    public static T With<T>(this T self, Action<T> set) {
      set.Invoke(self);
      return self;
    }

    public static T With<T>(this T self, Action<T> apply, bool when) {
      if (when) {
        apply(self);
      }

      return self;
    }
  }

  public static class UIExtensions {
    public static void AddListener(this Button self, UnityAction action) => self.onClick.AddListener(action);
    public static void RemoveListener(this Button self, UnityAction action) => self.onClick.RemoveListener(action);
    
    public static void RemoveAllListeners(this Button self) => self.onClick.RemoveAllListeners();
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