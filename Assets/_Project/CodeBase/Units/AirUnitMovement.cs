using CodeBase.Infrastructure.Services.StaticData.UnitData;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Units {
  public class AirUnitMovement : UnitMovement {
    public AirUnitMovement(Transform unitTransform, Transform modelTransform, UnitConfig config) : base(unitTransform, modelTransform, config) {}

    public override void PrepareIntro() {
      _positionFrom = _tileFrom.WorldPosition;
      _positionTo = _tileTo.WorldPosition;
      _unitTransform.localRotation = Quaternion.LookRotation(_positionTo - _positionFrom);
      _progressFactor = 0.1f * _speed;
    }
    
    public override Vector3 GetSmoothMovement() => Vector3.LerpUnclamped(new Vector3(_positionFrom.x, _unitTransform.transform.localPosition.y, _positionFrom.z), new Vector3(_positionTo.x, _unitTransform.transform.localPosition.y, _positionTo.z), _progress);

    protected override void PrepareOutro() => _progressFactor = Constants.ProgressFactorInitial * _speed;
  }
}