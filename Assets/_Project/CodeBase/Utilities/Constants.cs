namespace CodeBase.Utilities {
  public static class Constants {
    public const float UnitDirectionMultiplier = 0.8f;
    public const float ProgressFactorInitial = 1.5f;

    public class AssetsPath {
      public const string GameBoard = nameof(GameBoard);
    }

    public class ResourcesPath {
      public const string StaticData = nameof(StaticData);
      public const string UnitConfigs = nameof(UnitConfigs);
      public const string TileContentConfigs = nameof(TileContentConfigs);
    }

    public static class Boot {
      public const string Main = nameof(Main);
      public const string Bootstrap = nameof(Bootstrap);
      public const string FirstScenePath = "Assets/_Project/Scenes/Bootstrap.unity";
    }

    public static class Math {
      public const float Half = 0.5f;
      public const float QuarterTurn = 90.0f;
      public const float HalfTurn = 180.0f;
      public const float TripleQuarterTurn = 270.0f;
    }
  }
}