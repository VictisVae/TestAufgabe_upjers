using CodeBase.BoardContent;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.Player;
using CodeBase.Infrastructure.Services.StaticData.UnitData;
using UnityEngine;

namespace CodeBase.Units {
  public class AirUnit : UnitBase {
    public override void Construct(IGameFactory factory, IPlayerService playerService, UnitConfig config) {
      _unitFactory = factory;
      _unitMovement = new AirUnitMovement(transform, _model, config);
      float scale = config.Scale.RandomValueInRange;
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