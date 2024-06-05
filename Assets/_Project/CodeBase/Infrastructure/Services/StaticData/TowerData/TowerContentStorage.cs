using System.Collections.Generic;
using System.Linq;
using CodeBase.BoardContent;
using CodeBase.Tower;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData.TowerData {
  [CreateAssetMenu(fileName = nameof(TowerContentStorage), menuName = "Static Data/Tower Content Storage")]
  public class TowerContentStorage : EntityStaticData {
    private Dictionary<TowerType, TowerConfig> _towerMap;

    protected override void LoadData() {
      Debug.Log("TileContent Configs Loading ---STARTED---");
      _towerMap = Resources.LoadAll<TowerConfig>(Constants.ResourcesPath.TowerConfigs).ToDictionary(towerConfig => towerConfig.BuildScheme.TowerType);
    }

    public TowerConfig GetTowerConfig(TowerType towerType) => _towerMap[towerType];
  }
}