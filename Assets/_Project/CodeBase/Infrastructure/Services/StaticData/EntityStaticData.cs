using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData {
  public abstract class EntityStaticData : ScriptableObject {
    public void LoadEssential() => LoadData();
    protected virtual void LoadData() {}
  }
}