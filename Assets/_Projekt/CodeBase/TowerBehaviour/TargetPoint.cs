using CodeBase.Units;
using UnityEngine;

namespace CodeBase.TowerBehaviour {
  public class TargetPoint : MonoBehaviour {
    private void Awake() => Unit = transform.root.GetComponent<UnitBase>();
    public Vector3 Position => transform.localPosition;
    public UnitBase Unit { get; private set; }
  }
}