using CodeBase.BoardContent;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.Player;
using CodeBase.Infrastructure.Services.StaticData.UnitData;
using CodeBase.Tower;
using UnityEngine;

namespace CodeBase.Units {
  public abstract class UnitBase : GameBehaviour {
    [SerializeField]
    protected Transform _model;
    [field: SerializeField]
    public Target Target { get; private set; }
    protected UnitMovement _unitMovement;
    protected IGameFactory _unitFactory;
    protected IPlayerService _playerService;

    protected int _bringsGold;

    public override bool GameUpdate() {
      if (Target.IsAlive == false) {
        _playerService.AddCurrency(_bringsGold);
        Recycle();
        return false;
      }

      _unitMovement.UpdateProgress();

      while (_unitMovement.IsProgressExists) {
        if (_unitMovement.TileTo == null) {
          _playerService.TakeDamage();
          Recycle();
          return false;
        }

        _unitMovement.NullifyMovement();
        _unitMovement.PrepareNextState();
        _unitMovement.MultiplyProgress();
      }

      if (_unitMovement.NoDirectionChange) {
        transform.localPosition = _unitMovement.GetSmoothMovement();
      } else {
        transform.localRotation = _unitMovement.GetSmoothRotation();
      }

      return true;
    }

    public virtual void Construct(IGameFactory factory, IPlayerService playerService, UnitConfig config) {
      _playerService = playerService;
      _unitFactory = factory;
      _unitMovement = new UnitMovement(transform, _model, config);
      float scale = config.Scale.RandomValueInRange;
      _bringsGold = config.Gold;
      _model.localScale = new Vector3(scale, scale, scale);
      Target.Construct(config.Health);
    }

    public void SpawnItOn(BoardTile spawnPoint) {
      transform.localPosition = spawnPoint.transform.localPosition;
      _unitMovement.SetDirectionTiles(spawnPoint, spawnPoint.NextTileOnOnPath);
      _unitMovement.PrepareIntro();
    }
  }
}