using CodeBase.Factory;
using CodeBase.SceneCreation;
using UnityEngine;

namespace CodeBase.Units {
  public class AirUnit : UnitBase {
    public override void Initialise(UnitFactory factory, float scale, float pathOffset, float speed) {
      _unitFactory = factory;
      _unitMovement = new AirUnitMovement(transform, _model, pathOffset, speed);
      _model.localScale = new Vector3(scale, scale, scale);
    }

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
        transform.localRotation = _unitMovement.GetSmoothRotation();
      }

      return true;
    }

    public void SpawnItOn(BoardTile spawnPoint, BoardTile destinationPoint) {
      transform.localPosition = spawnPoint.transform.localPosition;
      _unitMovement.SetDirectionTiles(spawnPoint, destinationPoint);
      _unitMovement.PrepareIntro();
    }
  }
}