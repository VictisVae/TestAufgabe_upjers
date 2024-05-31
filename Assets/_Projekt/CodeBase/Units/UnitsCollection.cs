using System.Collections.Generic;
using CodeBase.TowerBehaviour;

namespace CodeBase.Units {
  public class UnitsCollection {
    private readonly List<UnitBase> _units = new List<UnitBase>();

    public void Add(UnitBase unit) {
      _units.Add(unit);
      Targets.Add(unit.TargetPoint);
    }

    public void GameUpdate() {
      for (int i = 0; i < _units.Count; i++) {
        if (_units[i].GameUpdate() == false) {
          int lastIndex = _units.Count - 1;
          _units[i] = _units[lastIndex];
          Targets[i] = Targets[lastIndex];
          _units.RemoveAt(lastIndex);
          Targets.RemoveAt(lastIndex);
          i -= 1;
        }
      }
    }

    public List<TargetPoint> Targets { get; } = new List<TargetPoint>();
  }
}