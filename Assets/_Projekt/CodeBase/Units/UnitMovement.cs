using CodeBase.Extensions;
using CodeBase.SceneCreation;
using UnityEngine;
using static CodeBase.Extensions.Constants.Math;

namespace CodeBase.Units {
  public class UnitMovement {
    private readonly Transform _modelTransform;
    private readonly Transform _unitTransform;
    private readonly float _pathOffset;
    private readonly float _speed;
    private BoardTile _tileFrom;
    private Direction _direction;
    private Vector3 _positionFrom, _positionTo;
    private float _directionAngleFrom, _directionAngleTo;
    private float _progressFactor;
    private float _progress;

    public UnitMovement(Transform unitTransform, Transform modelTransform, float pathOffset, float speed) {
      _unitTransform = unitTransform;
      _modelTransform = modelTransform;
      _pathOffset = pathOffset;
      _speed = speed;
    }

    public void SetDirectionTiles(BoardTile tileFrom, BoardTile tileTo) {
      _tileFrom = tileFrom;
      TileTo = tileTo;
      _progress = 0f;
    }

    public void PrepareIntro() {
      _positionFrom = _tileFrom.transform.localPosition;
      _positionTo = _tileFrom.ExitPoint;
      _direction = _tileFrom.PathDirection;
      DirectionChange = DirectionChange.None;
      _directionAngleFrom = _directionAngleTo = _direction.GetAngle();
      _modelTransform.localPosition = new Vector3(_pathOffset, 0.0f);
      _unitTransform.localRotation = _direction.GetRotation();
      _progressFactor = 2.0f * _speed;
    }

    public void PrepareNextState() {
      _tileFrom = TileTo;
      TileTo = TileTo.NextTileOnOnPath;
      _positionFrom = _positionTo;

      if (TileTo == null) {
        PrepareOutro();
        return;
      }

      _positionTo = _tileFrom.ExitPoint;
      DirectionChange = _direction.GetDirectionChangeTo(_tileFrom.PathDirection);
      _direction = _tileFrom.PathDirection;
      _directionAngleFrom = _directionAngleTo;

      switch (DirectionChange) {
        case DirectionChange.None:
          PrepareForward();
          break;
        case DirectionChange.TurnRight:
          PrepareTurnRight();
          break;
        case DirectionChange.TurnLeft:
          PrepareTurnLeft();
          break;
        case DirectionChange.TurnAround:
        default:
          PrepareTurnAround();
          break;
      }
    }

    public void UpdateProgress() => _progress += Time.deltaTime * _progressFactor;
    public Vector3 GetSmoothMovement() => Vector3.LerpUnclamped(_positionFrom, _positionTo, _progress);
    public float GetSmoothAngle() => Mathf.LerpUnclamped(_directionAngleFrom, _directionAngleTo, _progress);
    public void NullifyMovement() => _progress = (_progress - 1.0f) / _progressFactor;
    public void MultiplyProgress() => _progress *= _progressFactor;

    private void PrepareOutro() {
      _positionTo = _tileFrom.transform.localPosition;
      DirectionChange = DirectionChange.None;
      _directionAngleTo = _direction.GetAngle();
      _modelTransform.localPosition = new Vector3(_pathOffset, 0.0f);
      _unitTransform.localRotation = _direction.GetRotation();
      _progressFactor = 2.0f * _speed;
    }

    private void PrepareForward() {
      _unitTransform.localRotation = _direction.GetRotation();
      _directionAngleTo = _direction.GetAngle();
      _modelTransform.localPosition = new Vector3(_pathOffset, 0.0f);
    }

    private void PrepareTurnRight() {
      _directionAngleTo = _directionAngleFrom + QuarterTurn;
      _modelTransform.localPosition = new Vector3(_pathOffset - Half, 0);
      _unitTransform.localPosition = _positionFrom + _direction.GetHalfVector();
      _progressFactor = _speed / (Mathf.PI * Half * (Half - _pathOffset));
    }

    private void PrepareTurnLeft() {
      _directionAngleTo = _directionAngleFrom - QuarterTurn;
      _modelTransform.localPosition = new Vector3(_pathOffset + Half, 0);
      _unitTransform.localPosition = _positionFrom + _direction.GetHalfVector();
      _progressFactor = _speed / (Mathf.PI * Half * (Half - _pathOffset));
    }

    private void PrepareTurnAround() {
      _directionAngleTo = _directionAngleFrom + (_pathOffset < 0.0f ? HalfTurn : -HalfTurn);
      _modelTransform.localPosition = new Vector3(_pathOffset, 0);
      _unitTransform.localPosition = _positionFrom;
      _progressFactor = _speed / (Mathf.PI * Mathf.Max(Mathf.Abs(_pathOffset), 0.2f));
    }

    public BoardTile TileTo { get; private set; }
    public DirectionChange DirectionChange { get; private set; }
    public bool IsProgressExists => _progress >= 1;
  }
}