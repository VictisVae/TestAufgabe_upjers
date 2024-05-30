using CodeBase.Extensions;
using CodeBase.Factory;
using CodeBase.SceneCreation;
using UnityEngine;

namespace CodeBase.Units {
  public class Unit : MonoBehaviour {
    [SerializeField]
    private Transform _model;
    private BoardTile _tileFrom, _tileTo;
    private Vector3 _positionFrom, _positionTo;
    private float _progress, _progressFactor;
    private Direction _direction;
    private DirectionChange _directionChange;
    private float _directionAngleFrom, _directionAngleTo;
    private float _pathOffset;
    private float _speed;

    public void Initialise(float scale, float pathOffset, float speed) {
      _model.localScale = new Vector3(scale, scale, scale);
      _pathOffset = pathOffset;
      _speed = speed;
    }

    public void SpawnOn(BoardTile spawnPoint) {
      transform.localPosition = spawnPoint.transform.localPosition;
      _tileFrom = spawnPoint;
      _tileTo = spawnPoint.NextTileOnOnPath;
      _progress = 0;
      PrepareIntro();
    }

    public bool GameUpdate() {
      _progress += Time.deltaTime * _progressFactor;

      while (_progress >= 1) {
        if (_tileTo == null) {
          UnitFactory.Reclaim(this);
          return false;
        }

        _progress = (_progress - 1.0f) / _progressFactor;
        PrepareNextState();
        _progress *= _progressFactor;
      }

      if (_directionChange == DirectionChange.None) {
        transform.localPosition = Vector3.LerpUnclamped(_positionFrom, _positionTo, _progress);
      } else {
        float angle = Mathf.LerpUnclamped(_directionAngleFrom, _directionAngleTo, _progress);
        transform.localRotation = Quaternion.Euler(0.0f, angle, 0.0f);
      }

      return true;
    }

    private void PrepareIntro() {
      _positionFrom = _tileFrom.transform.localPosition;
      _positionTo = _tileFrom.ExitPoint;
      _direction = _tileFrom.PathDirection;
      _directionChange = DirectionChange.None;
      _directionAngleFrom = _directionAngleTo = _direction.GetAngle();
      _model.localPosition = new Vector3(_pathOffset, 0.0f);
      transform.localRotation = _direction.GetRotation();
      _progressFactor = 2.0f * _speed;
    }

    private void PrepareOutro() {
      _positionTo = _tileFrom.transform.localPosition;
      _directionChange = DirectionChange.None;
      _directionAngleTo = _direction.GetAngle();
      _model.localPosition = new Vector3(_pathOffset, 0.0f);
      transform.localRotation = _direction.GetRotation();
      _progressFactor = 2.0f * _speed;
    }

    private void PrepareNextState() {
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
      transform.localRotation = _direction.GetRotation();
      _directionAngleTo = _direction.GetAngle();
      _model.localPosition = new Vector3(_pathOffset, 0.0f);
    }

    private void PrepareTurnRight() {
      _directionAngleTo = _directionAngleFrom + 90.0f;
      _model.localPosition = new Vector3(_pathOffset - 0.5f, 0);
      transform.localPosition = _positionFrom + _direction.GetHalfVector();
      _progressFactor = _speed / (Mathf.PI * 0.5f * (0.5f - _pathOffset));
    }

    private void PrepareTurnLeft() {
      _directionAngleTo = _directionAngleFrom - 90.0f;
      _model.localPosition = new Vector3(_pathOffset + 0.5f, 0);
      transform.localPosition = _positionFrom + _direction.GetHalfVector();
      _progressFactor = _speed / (Mathf.PI * 0.5f * (0.5f - _pathOffset));
    }

    private void PrepareTurnAround() {
      _directionAngleTo = _directionAngleFrom + (_pathOffset < 0.0f ? 180.0f : -180.0f);
      _model.localPosition = new Vector3(_pathOffset, 0);
      transform.localPosition = _positionFrom;
      _progressFactor = _speed / (Mathf.PI * Mathf.Max(Mathf.Abs(_pathOffset), 0.2f));
    }

    public UnitFactory UnitFactory { get; set; }
  }
}