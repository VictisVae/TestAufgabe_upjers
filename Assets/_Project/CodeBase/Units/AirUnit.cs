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
      _playerService = playerService;
      float scale = config.Scale.RandomValueInRange;
      _bringsGold = config.Gold;
      _type = UnitType.Air;
      _model.localScale = new Vector3(scale, scale, scale);
      Target.Construct(config.Health);
    }

    public override void Recycle() => _unitFactory.Reclaim(this, _type);

    public void SpawnItOn(BoardTile spawnPoint, BoardTile destinationPoint) {
      Vector3 transformLocalPosition = spawnPoint.transform.localPosition;
      transform.localPosition = new Vector3(transformLocalPosition.x, transform.localPosition.y, transformLocalPosition.z) ;
      _unitMovement.SetDirectionTiles(spawnPoint, destinationPoint);
      _unitMovement.PrepareIntro();
    }
  }
}