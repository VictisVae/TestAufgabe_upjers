using CodeBase.Infrastructure.Pool;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Infrastructure.Factory {
  public abstract class FactoryObject : MonoBehaviour, IPoolable {
    public void Get() => gameObject.Enable();

    public void Return() => gameObject.Disable();
  }
}