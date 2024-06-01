using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Infrastructure.Factory {
  public abstract class FactoryObject : MonoBehaviour {
    public void Get() => gameObject.Enable();
  }
}