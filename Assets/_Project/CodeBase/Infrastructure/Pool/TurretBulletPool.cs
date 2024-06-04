using System;

namespace CodeBase.Infrastructure.Pool {
  public class TurretBulletPool : ObjectPool<TurretBullet> {
    public TurretBulletPool(Func<TurretBullet> objectGenerator) : base(objectGenerator) {}
  }
}