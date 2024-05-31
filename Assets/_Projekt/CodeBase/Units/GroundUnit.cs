using CodeBase.SceneCreation;
using UnityEngine;

namespace CodeBase.Units {
  public class GroundUnit : UnitBase {
    public override bool GameUpdate() {
      _unitMovement.UpdateProgress();

      while (_unitMovement.IsProgressExists) {
        if (_unitMovement.TileTo == null) {
          _unitFactory.Reclaim(this);
          return false;
        }

        _unitMovement.NullifyMovement();
        _unitMovement.PrepareNextState();
        _unitMovement.MultiplyProgress();
      }

      if (_unitMovement.DirectionChange == DirectionChange.None) {
        transform.localPosition = _unitMovement.GetSmoothMovement();
      } else {
        float angle = _unitMovement.GetSmoothAngle();
        transform.localRotation = Quaternion.Euler(0.0f, angle, 0.0f);
      }

      return true;
    }
  }
}