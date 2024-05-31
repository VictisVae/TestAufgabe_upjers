using System.Collections.Generic;
using CodeBase.TowerBehaviour;

namespace CodeBase.Units {
  public class BehavioursCollection {
    private readonly List<GameBehaviour> _behaviours = new List<GameBehaviour>();

    public void Add(UnitBase unit) {
      _behaviours.Add(unit);
      Targets.Add(unit.TargetPoint);
    }

    public void GameUpdate() {
      for (int i = 0; i < _behaviours.Count; i++) {
        if (_behaviours[i].GameUpdate() == false) {
          int lastIndex = _behaviours.Count - 1;
          _behaviours[i] = _behaviours[lastIndex];
          Targets[i] = Targets[lastIndex];
          _behaviours.RemoveAt(lastIndex);
          Targets.RemoveAt(lastIndex);
          i -= 1;
        }
      }
    }

    public void Clear() {
      foreach (GameBehaviour behaviour in _behaviours) {
        behaviour.Recycle();
      }

      _behaviours.Clear();
    }

    public bool IsEmpty => _behaviours.Count == 0;

    public List<TargetPoint> Targets { get; } = new List<TargetPoint>();
  }
}