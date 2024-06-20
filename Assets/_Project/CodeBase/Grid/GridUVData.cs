using UnityEngine;

namespace CodeBase.Grid {
  public readonly struct GridUVData {
    private readonly int[] _uvIndexes;
    public GridUVData(int[] uvIndexes) => _uvIndexes = uvIndexes;
    public int[] GetUVIndexes => _uvIndexes;

    public static Vector2[] GetUVDirectionUp() =>
      new[] {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(1, 1)
      };

    public static Vector2[] GetUVDirectionRight() =>
      new[] {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(1, 1)
      };

    public static Vector2[] GetUVDirectionDown() =>
      new[] {
        new Vector2(1, 1),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, 0)
      };

    public static Vector2[] GetUVDirectionLeft() =>
      new[] {
        new Vector2(1, 1),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(0, 0)
      };
  }
}