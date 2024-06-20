using CodeBase.Grid;
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
      Target.NoHealthEvent += OnUnitDies;
    }

    public override void Recycle() => _unitFactory.Reclaim(this, _type);

    public void SpawnItOn(GridTile spawnPoint, GridTile destinationPoint) {
      Vector3 transformLocalPosition = spawnPoint.WorldPosition;
      transform.localPosition = new Vector3(transformLocalPosition.x, transform.localPosition.y, transformLocalPosition.z) ;
      _unitMovement.SetDirectionTiles(spawnPoint, destinationPoint);
      _unitMovement.PrepareIntro();
    }
  }
}