﻿using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Infrastructure.Services.StaticData.UnitData;
using UnityEngine;

namespace CodeBase.Units {
  public class AirUnitMovement : UnitMovement {
    public AirUnitMovement(Transform unitTransform, Transform modelTransform, UnitConfig config) : base(unitTransform, modelTransform, config) {}

    public override void PrepareIntro() {
      _positionFrom = _tileFrom.transform.localPosition;
      _positionTo = _tileTo.transform.localPosition;
      _unitTransform.localRotation = Quaternion.LookRotation(_positionTo - _positionFrom);
      _progressFactor = 0.1f * _speed;
    }

    protected override void PrepareOutro() => _progressFactor = 2.0f * _speed;
  }
}