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
    
    public override void Recycle() => _unitFactory.Reclaim(this);

    public void SpawnItOn(BoardTile spawnPoint, BoardTile destinationPoint) {
      transform.localPosition = spawnPoint.transform.localPosition;
      _unitMovement.SetDirectionTiles(spawnPoint, destinationPoint);
      _unitMovement.PrepareIntro();
    }
  }
}