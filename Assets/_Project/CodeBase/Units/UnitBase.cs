using CodeBase.Grid;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.Player;
using CodeBase.Infrastructure.Services.StaticData.UnitData;
using CodeBase.Towers;
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

    protected UnitType _type;
    protected int _bringsGold;

    public override bool GameUpdate() {
      if (Target.NoHealth) {
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
      _type = config.Type;
      _model.localScale = new Vector3(scale, scale, scale);
      Target.Construct(config.Health);
      Target.NoHealthEvent += OnUnitDies;
    }

    protected void OnUnitDies() {
      Recycle();
      _playerService.AddCurrency(_bringsGold);
      Target.NoHealthEvent -= OnUnitDies;
    }

    public void SpawnItOn(GridTile spawnPoint) {
      transform.localPosition = spawnPoint.WorldPosition;
      _unitMovement.SetDirectionTiles(spawnPoint, spawnPoint.NextTileOnOnPath);
      _unitMovement.PrepareIntro();
    }
  }
}