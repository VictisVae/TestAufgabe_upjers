using UnityEngine;

namespace CodeBase.Tower {
  [RequireComponent(typeof(Tower))]
  public class TowerVisualRadius : MonoBehaviour {
    [SerializeField]
    private RadiusObject _radius;
    [SerializeField]
    private Transform _turretBase;
    [SerializeField]
    private float _radiusHeight = 0.3f;
    private float _targetingRange;

    private void Awake() {
      _targetingRange = GetComponent<Tower>().TargetingRange;
      PlaceRadius();
      HideRadius();
    }

    private void PlaceRadius() {
      _radius.transform.localPosition = new Vector3(_turretBase.position.x, _turretBase.position.y + _radiusHeight, _turretBase.position.z);
      float towerTargetingRange = _targetingRange * 2;
      _radius.transform.localScale = new Vector3(towerTargetingRange, 0, towerTargetingRange);
    }

    public void SwitchRadiusColor(Color color) => _radius.SwitchColor(color);
    public void ResetRadiusColor() => _radius.ResetColor();
    public void ShowRadius() => _radius.Enable();
    public void HideRadius() => _radius.Disable();
  }
}