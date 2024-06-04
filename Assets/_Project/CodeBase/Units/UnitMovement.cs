using CodeBase.BoardContent;
using CodeBase.Infrastructure.Services.StaticData.UnitData;
using CodeBase.Utilities;
using UnityEngine;
using static CodeBase.Utilities.Constants;
using static CodeBase.Utilities.Constants.Math;

namespace CodeBase.Units {
  public class UnitMovement {
    protected readonly Transform _unitTransform;
    protected readonly float _speed;
    protected float _progress;
    private readonly Transform _modelTransform;
    private readonly float _pathOffset;
    protected Vector3 _positionFrom, _positionTo;
    protected float _progressFactor;
    protected BoardTile _tileFrom, _tileTo;
    private Direction _direction;
    private DirectionChange _directionChange;
    private float _directionAngleFrom, _directionAngleTo;

    public UnitMovement(Transform unitTransform, Transform modelTransform, UnitConfig config) {
      _unitTransform = unitTransform;
      _modelTransform = modelTransform;
      _pathOffset = config.PathOffset.RandomValueInRange;
      _speed = config.Speed.RandomValueInRange;
    }

    public void SetDirectionTiles(BoardTile tileFrom, BoardTile tileTo) {
      _tileFrom = tileFrom;
      _tileTo = tileTo;
      _progress = 0f;
    }

    public virtual void PrepareIntro() {
      _positionFrom = _tileFrom.transform.localPosition;
      _positionTo = _tileFrom.ExitPoint;
      _direction = _tileFrom.PathDirection;
      _directionChange = DirectionChange.None;
      _directionAngleFrom = _directionAngleTo = _direction.GetAngle();
      _modelTransform.localPosition = new Vector3(_pathOffset, 0.0f);
      _unitTransform.localRotation = _direction.GetRotation();
      _progressFactor = ProgressFactorInitial * _speed;
    }

    public void PrepareNextState() {
      _tileFrom = _tileTo;
      _tileTo = _tileTo.NextTileOnOnPath;
      _positionFrom = _positionTo;

      if (_tileTo == null) {
        PrepareOutro();
        return;
      }

      _positionTo = _tileFrom.ExitPoint;
      _directionChange = _direction.GetDirectionChangeTo(_tileFrom.PathDirection);
      _direction = _tileFrom.PathDirection;
      _directionAngleFrom = _directionAngleTo;
      HandleDirection();
    }

    public void UpdateProgress() => _progress += Time.deltaTime * _progressFactor;
    public virtual Vector3 GetSmoothMovement() => Vector3.LerpUnclamped(_positionFrom, _positionTo, _progress);
    public Quaternion GetSmoothRotation() => Quaternion.Euler(0.0f, SmoothAngle, 0.0f);
    public void NullifyMovement() => _progress = (_progress - 1.0f) / _progressFactor;
    public void MultiplyProgress() => _progress *= _progressFactor;

    protected virtual void PrepareOutro() {
      _positionTo = _tileFrom.transform.localPosition;
      _directionChange = DirectionChange.None;
      _directionAngleTo = _direction.GetAngle();
      _modelTransform.localPosition = new Vector3(_pathOffset, 0.0f);
      _unitTransform.localRotation = _direction.GetRotation();
      _progressFactor = ProgressFactorInitial * _speed;
    }

    private void HandleDirection() {
      switch (_directionChange) {
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

    private void PrepareForward() {
      _unitTransform.localRotation = _direction.GetRotation();
      _directionAngleTo = _direction.GetAngle();
      _modelTransform.localPosition = new Vector3(_pathOffset, 0.0f);
    }

    private void PrepareTurnRight() {
      _directionAngleTo = _directionAngleFrom + QuarterTurn;
      _modelTransform.localPosition = new Vector3(_pathOffset - Half, 0);
      _unitTransform.localPosition = _positionFrom + _direction.GetHalfVector();
      _progressFactor = _speed * UnitDirectionMultiplier;
    }

    private void PrepareTurnLeft() {
      _directionAngleTo = _directionAngleFrom - QuarterTurn;
      _modelTransform.localPosition = new Vector3(_pathOffset + Half, 0);
      _unitTransform.localPosition = _positionFrom + _direction.GetHalfVector();
      _progressFactor = _speed * UnitDirectionMultiplier;
    }

    private void PrepareTurnAround() {
      _directionAngleTo = _directionAngleFrom + (_pathOffset < 0.0f ? HalfTurn : -HalfTurn);
      _modelTransform.localPosition = new Vector3(_pathOffset, 0);
      _unitTransform.localPosition = _positionFrom;
      _progressFactor = _speed * UnitDirectionMultiplier;
    }

    public BoardTile TileTo => _tileTo;
    public bool NoDirectionChange => _directionChange == DirectionChange.None;
    public bool IsProgressExists => _progress >= 1;
    private float SmoothAngle => Mathf.LerpUnclamped(_directionAngleFrom, _directionAngleTo, _progress);
  }
}