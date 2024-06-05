using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData {
  public class StaticDataService : IStaticDataService {
    private Dictionary<Type, EntityStaticData> _staticDataMap;

    public void Initialize() {
      Debug.Log("Static data initialization ---STARTED---");
      _staticDataMap = Resources.LoadAll<EntityStaticData>(Constants.ResourcesPath.StaticData).ToDictionary(x => x.GetType(), x => x);

      foreach (EntityStaticData staticData in _staticDataMap.Values) {
        Debug.Log(staticData.name + " - ---INITIALIZED---");
        staticData.LoadEssential();
      }

      Debug.Log("Static data initialization ---COMPLETED---");
    }

    public T GetStaticData<T>() where T : EntityStaticData => _staticDataMap[typeof(T)] as T;
  }
}