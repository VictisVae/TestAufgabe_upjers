using System.Collections.Generic;
using System.Linq;
using CodeBase.SceneCreation;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData {
  [CreateAssetMenu(fileName = nameof(TileContentStorage), menuName = "Static Data/Tile Content Storage")]
  public class TileContentStorage : EntityStaticData {
    private Dictionary<TileContentType, TileContentConfig> _tileContentMap;

    protected override void LoadData() {
      Debug.Log("TileContent Configs Loading ---STARTED---");

      _tileContentMap = Resources.LoadAll<TileContentConfig>(Constants.ResourcesPath.TileContentConfigs)
        .ToDictionary(tileContentConfig => tileContentConfig.Type);
    }

    public TileContent GetTileContent(TileContentType tileType) => _tileContentMap[tileType].Prefab;
  }
}