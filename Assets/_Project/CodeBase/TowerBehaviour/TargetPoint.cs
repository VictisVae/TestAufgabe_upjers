using UnityEngine;

namespace CodeBase.TowerBehaviour {
  public class TargetPoint : MonoBehaviour {
    public Vector3 Position => transform.localPosition;
  }
}