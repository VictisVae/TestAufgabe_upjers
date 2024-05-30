using System.Collections.Generic;

namespace CodeBase.Units {
  public class UnitsCollection {
    private List<Unit> _units = new List<Unit>();

    public void Add(Unit unit) {
      _units.Add(unit);
    }

    public void GameUpdate() {
      for (int i = 0; i < _units.Count; i++) {
        if (_units[i].GameUpdate() == false) {
          int lastIndex = _units.Count - 1;
          _units[i] = _units[lastIndex];
          _units.RemoveAt(lastIndex);
          i -= 1;
        }
      }
    }
  }
}