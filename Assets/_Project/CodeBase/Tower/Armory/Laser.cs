using UnityEngine;

namespace CodeBase.Tower.Armory {
  [RequireComponent(typeof(LineRenderer))]
  public class Laser : MonoBehaviour {
    private LineRenderer _lineRenderer;
    [SerializeField]
    private float _laserDuration = 0.05f;

    private float _viewDuration;

    private void Awake() {
      _lineRenderer = GetComponent<LineRenderer>();
      _lineRenderer.enabled = false;
    }

    private Vector3 LaserOriginPosition => _lineRenderer.transform.position;

    public void Blast(Vector3 targetPosition) {
      _lineRenderer.SetPosition(0, LaserOriginPosition);
      _lineRenderer.SetPosition(1, targetPosition);
      _lineRenderer.enabled = true;
      _viewDuration = _laserDuration;
    }

    public void UpdateLaser() {
      while (_viewDuration > 0) {
        _viewDuration -= Time.deltaTime;
        return;
      }

      _lineRenderer.enabled = false;
    }
  }
}