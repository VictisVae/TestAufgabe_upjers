using CodeBase.Utilities;
using UnityEngine;
namespace CodeBase.BoardContent {
  [RequireComponent(typeof(Tower))]
  public class TowerVisualRadius : MonoBehaviour {
    [SerializeField]
    private GameObject _radiusPrefab;
    [SerializeField]
    private Transform _turretBase;
    [SerializeField]
    private float _radiusHeight = 0.3f;
    private Tower _tower;
    private Transform _radius;

    private void Awake() {
      _tower = GetComponent<Tower>();
      PlaceRadius();
      HideRadius();
    }

    private void PlaceRadius() {
      var radius = Instantiate(_radiusPrefab, transform);
      radius.transform.localPosition = new Vector3(_turretBase.position.x, _turretBase.position.y + _radiusHeight, _turretBase.position.z) ;
      float towerTargetingRange = _tower.TargetingRange * 2;
      radius.transform.localScale = new Vector3(towerTargetingRange, 0, towerTargetingRange);
      _radius = radius.transform;
    }

    public void ShowRadius() => _radius.Enable();
    public void HideRadius() => _radius.Disable();
  }
}