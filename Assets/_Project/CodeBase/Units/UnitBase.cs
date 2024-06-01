using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.SceneCreation;
using CodeBase.TowerBehaviour;
using UnityEngine;

namespace CodeBase.Units {
  public abstract class UnitBase : GameBehaviour {
    [SerializeField]
    protected Transform _model;
    protected UnitMovement _unitMovement;
    protected IGameFactory _unitFactory;
    private void Awake() => TargetPoint = GetComponent<TargetPoint>();

    public virtual void Construct(IGameFactory factory, UnitConfig config) {
      _unitFactory = factory;
      _unitMovement = new UnitMovement(transform, _model, config);
      float scale = config.Scale.RandomValueInRange;
      _model.localScale = new Vector3(scale, scale, scale);
    }

    public override bool GameUpdate() {
      _unitMovement.UpdateProgress();

      while (_unitMovement.IsProgressExists) {
        if (_unitMovement.TileTo == null) {
          //TODO Implement Player -- Health on enemy destintaion reach
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

    public void SpawnItOn(BoardTile spawnPoint) {
      transform.localPosition = spawnPoint.transform.localPosition;
      _unitMovement.SetDirectionTiles(spawnPoint, spawnPoint.NextTileOnOnPath);
      _unitMovement.PrepareIntro();
    }

    public TargetPoint TargetPoint { get; private set; }
  }
}