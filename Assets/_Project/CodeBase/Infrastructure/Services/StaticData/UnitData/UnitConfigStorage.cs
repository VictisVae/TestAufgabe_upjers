using System.Collections.Generic;
using System.Linq;
using CodeBase.Units;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData.UnitData {
  [CreateAssetMenu(fileName = nameof(UnitConfigStorage), menuName = "Static Data/Unit Config Storage")]
  public class UnitConfigStorage : EntityStaticData {
    private Dictionary<UnitType, UnitConfig> _unitConfigsMap;

    protected override void LoadData() {
      Debug.Log("Unit Configs Loading ---STARTED---");
      _unitConfigsMap = Resources.LoadAll<UnitConfig>(Constants.ResourcesPath.UnitConfigs).ToDictionary(unitConfig => unitConfig.Type);
    }

    public UnitConfig GetUnitConfig(UnitType unitType) => _unitConfigsMap[unitType];
  }
}