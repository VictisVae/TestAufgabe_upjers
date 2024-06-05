using System;
using System.Collections.Generic;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Tower.Armory {
  public class TowerLaser : MonoBehaviour {
    [SerializeField]
    private Transform _turretHead;
    [SerializeField]
    private Transform _turretPlatform;
    [SerializeField]
    private Laser[] _lasers;
    private Func<List<Target>> _targets;
    private Target _currentTarget;
    private IMonoEventsProvider _monoEventsProvider;
    private int _damage;
    private float _reloadTime;
    private float _shootFrequency;
    private float _delayLeft;
    private float _targetingRange;
    private void Awake() => _monoEventsProvider = GlobalService.Container.GetSingle<IMonoEventsProvider>();
    private void OnEnable() => _monoEventsProvider.OnApplicationUpdateEvent += MonoUpdate;
    private void OnDisable() => _monoEventsProvider.OnApplicationUpdateEvent -= MonoUpdate;

    public void Shoot() {
      Vector3 point = _currentTarget.Position;
      _turretPlatform.LookAt(new Vector3(point.x, _turretPlatform.transform.position.y, point.z));
      _turretHead.LookAt(new Vector3(point.x, point.y + Constants.Math.Half, point.z));

      while (_reloadTime > 0) {
        _reloadTime -= Time.deltaTime;
        return;
      }

      _reloadTime = _shootFrequency;

      foreach (Laser laser in _lasers) {
        laser.Blast(_currentTarget.Position);

        if (_currentTarget != null) {
          _currentTarget.GetHit(_damage);
        }
      }
    }

    public void ReceiveTargets(Func<List<Target>> targets) => _targets = targets;

    public bool IsTargetAcquired() {
      _currentTarget = FindTarget();
      return _currentTarget != null;
    }

    public bool IsTargetTracked() {
      if (_currentTarget == null) {
        return false;
      }

      Vector3 turretPosition = transform.parent.position;

      if (CalculateDistance(turretPosition, _currentTarget.Position) > _targetingRange) {
        _currentTarget = null;
        return false;
      }

      return true;
    }

    public void Construct(float configShootFrequency, int configLaserDamage, float targetingRange) {
      _shootFrequency = configShootFrequency;
      _damage = configLaserDamage;
      _targetingRange = targetingRange;
    }

    private void MonoUpdate() {
      foreach (var laser in _lasers) {
        laser.UpdateLaser();
      }
    }

    private float CalculateDistance(Vector3 turretPosition, Vector3 targetPosition) => Vector3.Distance(turretPosition, targetPosition);

    private Target FindTarget() {
      List<Target> targets = _targets.Invoke();
      Vector3 turretPosition = transform.parent.position;

      foreach (Target target in targets) {
        if (CalculateDistance(turretPosition, target.Position) < _targetingRange) {
          return target;
        }
      }

      return null;
    }
  }
}