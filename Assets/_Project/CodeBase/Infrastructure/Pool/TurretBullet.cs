using System;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Tower;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Infrastructure.Pool {
  public class TurretBullet : MonoBehaviour, IPoolable {
    private IMonoEventsProvider _monoEventsProvider;
    private Action<TurretBullet> _onTargetReachAction;
    private Target _target;
    private Vector3 _lastIdentifiedPosition;
    private float _speed;

    private void Awake() => _monoEventsProvider = GlobalService.Container.GetSingle<IMonoEventsProvider>();

    private void OnEnable() => _monoEventsProvider.OnApplicationUpdateEvent += MonoUpdate;

    private void OnDisable() => _monoEventsProvider.OnApplicationUpdateEvent -= MonoUpdate;

    private void MonoUpdate() {
      if (_target != null) {
        _lastIdentifiedPosition = _target.transform.localPosition;
      }
      
      MoveBullet();

      if (Vector3.Distance(transform.localPosition, _lastIdentifiedPosition) > 0.1f) {
        return;
      }

      _onTargetReachAction(this);
    }

    public void Get() => gameObject.Enable();
    public void Return() => gameObject.Disable();
    public void SetBulletPosition(Vector3 position) => transform.localPosition = position;
    public void SetBulletSpeed(float speed) => _speed = speed;
    public void SetTarget(Target target) {
      _target = target;
      _lastIdentifiedPosition = _target.transform.localPosition;
    }

    public void Shoot(Action<TurretBullet> onTargetReach) => _onTargetReachAction = onTargetReach;

    private void MoveBullet() {
      Vector3 shootDirection = (_lastIdentifiedPosition - transform.localPosition).normalized;
      Vector3 newBulletPosition = _speed * Time.deltaTime * shootDirection;
      transform.localPosition += newBulletPosition;
    }
  }
}