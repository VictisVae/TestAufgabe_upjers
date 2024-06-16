using System;
using CodeBase.BoardContent;
using CodeBase.Towers;
using CodeBase.Units;

namespace CodeBase.Infrastructure.Pool {
  public class TowerPool : ObjectPool<Tower> {
    public TowerPool(Func<Tower> objectGenerator) : base(objectGenerator) {}
  }

  public class TilePool : ObjectPool<TileContent> {
    public TilePool(Func<TileContent> objectGenerator) : base(objectGenerator) {}
  }

  public class UnitPool : ObjectPool<UnitBase> {
    public UnitPool(Func<UnitBase> objectGenerator) : base(objectGenerator) {}
  }
}