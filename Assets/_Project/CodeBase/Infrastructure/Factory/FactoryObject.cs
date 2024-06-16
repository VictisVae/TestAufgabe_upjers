using CodeBase.Infrastructure.Pool;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Infrastructure.Factory {
  public abstract class FactoryObject : MonoBehaviour, IPoolable {
    public abstract void Recycle();
    public virtual void ViewAvailable(bool isAvailable) {}
    public virtual void SetNormal() {}
    public void Get() => gameObject.Enable();
    public void Return() => gameObject.Disable();
  }
}