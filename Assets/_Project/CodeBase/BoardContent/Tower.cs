using System;
using System.Collections.Generic;
using CodeBase.TowerBehaviour;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.BoardContent {
  [RequireComponent(typeof(TargetPoint))]
  public class Tower : TileContent {
    [SerializeField]
    [Range(1.5f, 10.5f)]
    private float _targetingRange = 1.5f;
    [SerializeField]
    private Transform _turret;
    [SerializeField]
    private Transform[] _laserBeams;
    private Func<List<TargetPoint>> _targets;
    private TargetPoint _target;
    private Vector3 _laserBeamScale;
    private void Awake() => _laserBeamScale = _laserBeams[0].localScale;
    
    public override void GameUpdate() {
      if (IsTargetTracked() || IsTargetAcquired()) {
        Shoot();
      }
    }

    public void ReceiveTargets(Func<List<TargetPoint>> targets) => _targets = targets;

    private bool IsTargetAcquired() {
      _target = FindTarget();
      return _target != null;
    }

    private void Shoot() {
      var point = _target.Position;
      _turret.LookAt(point);

      foreach (Transform laserBeam in _laserBeams) {
        laserBeam.localRotation = _turret.localRotation;
      }

      var distance = CalculateDistance(_turret.position, point);
      _laserBeamScale.z = distance;

      foreach (Transform laserBeam in _laserBeams) {
        laserBeam.localScale = _laserBeamScale;
        laserBeam.localPosition = _turret.localPosition + Constants.Math.Half * distance * laserBeam.forward;
      }
    }

    private TargetPoint FindTarget() {
      List<TargetPoint> targets = _targets.Invoke();
      Vector3 turretPosition = transform.localPosition;

      foreach (TargetPoint target in targets) {
        if (CalculateDistance(turretPosition, target.Position) < _targetingRange) {
          return target;
        }
      }

      return null;
    }

    private bool IsTargetTracked() {
      if (_target == null) {
        return false;
      }

      Vector3 turretPosition = transform.localPosition;

      if (CalculateDistance(turretPosition, _target.Position) > _targetingRange) {
        _target = null;
        return false;
      }

      return true;
    }

    private float CalculateDistance(Vector3 turretPosition, Vector3 targetPosition) => Vector3.Distance(turretPosition, targetPosition);

    private void OnDrawGizmos() {
      Gizmos.color = Color.yellow;
      Vector3 position = _turret.position;
      position.y += 0.01f;
      Gizmos.DrawWireSphere(position, _targetingRange);

      if (_target != null) {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(position, _target.transform.position);
      }
    }
  }
}