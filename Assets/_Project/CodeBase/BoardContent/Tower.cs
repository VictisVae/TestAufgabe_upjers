using System;
using System.Collections.Generic;
using CodeBase.Infrastructure.Pool;
using CodeBase.Infrastructure.Services.StaticData.TowerData;
using CodeBase.TowerBehaviour;
using UnityEngine;

namespace CodeBase.BoardContent {
  public class Tower : TileContent {
    [SerializeField]
    private Transform _turretPlatform;
    [SerializeField]
    private Transform _turretHead;
    [SerializeField]
    private Transform[] _bulletSpawnPoint;
    [SerializeField]
    [Range(1f, 10f)]
    private float _targetingRange = 1.5f;
    private float _reloadTime;
    private float _shootFrequency;
    private int _damage;
    private Func<List<Target>> _targets;
    private Target _target;

    public void Construct(TowerConfig config) {
      _shootFrequency = config.ShootFrequency;
      _damage = config.BulletDamage;
    }

    public override void GameUpdate() {
      if (IsTargetTracked() || IsTargetAcquired()) {
        Shoot();
      }
    }

    public void ReceiveTargets(Func<List<Target>> targets) => _targets = targets;

    private bool IsTargetAcquired() {
      _target = FindTarget();
      return _target != null;
    }

    private void Shoot() {
      Vector3 point = _target.Position;
      _turretPlatform.LookAt(new Vector3(point.x, _turretPlatform.transform.position.y, point.z));
      _turretHead.LookAt(new Vector3(point.x, point.y, point.z));

      while (_reloadTime > 0) {
        _reloadTime -= Time.deltaTime;
        return;
      }

      _reloadTime = _shootFrequency;

      foreach (Transform bulletSpawnPoints in _bulletSpawnPoint) {
        TurretBullet bullet = _gameFactory.CreateBullet();
        bullet.SetBulletPosition(bulletSpawnPoints.position);
        bullet.SetTarget(_target);
        bullet.Shoot(HitTarget);
      }
    }

    private void HitTarget(TurretBullet bullet) {
      _gameFactory.ReclaimBullet(bullet);

      if (_target != null) {
        _target.GetHit(_damage);
      }
    }

    private Target FindTarget() {
      List<Target> targets = _targets.Invoke();
      Vector3 turretPosition = transform.localPosition;

      foreach (Target target in targets) {
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
      Vector3 position = _turretPlatform.position;
      position.y += 0.01f;
      Gizmos.DrawWireSphere(position, _targetingRange);

      if (_target != null) {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(position, _target.transform.position);
      }
    }
  }
}