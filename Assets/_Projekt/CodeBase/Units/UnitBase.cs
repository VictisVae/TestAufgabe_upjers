using CodeBase.Factory;
using CodeBase.SceneCreation;
using UnityEngine;

namespace CodeBase.Units {
  public abstract class UnitBase : MonoBehaviour {
    [SerializeField]
    protected Transform _model;
    
    protected UnitMovement _unitMovement;
    protected UnitFactory _unitFactory;

    public virtual void Initialise(UnitFactory factory, float scale, float pathOffset, float speed) {
      _unitFactory = factory;
      _unitMovement = new UnitMovement(transform, _model, pathOffset, speed);
      _model.localScale = new Vector3(scale, scale, scale);
    }

    public void SpawnItOn(BoardTile spawnPoint) {
      transform.localPosition = spawnPoint.transform.localPosition;
      _unitMovement.SetDirectionTiles(spawnPoint, spawnPoint.NextTileOnOnPath);
      _unitMovement.PrepareIntro();
    }

    public abstract bool GameUpdate();
  }
}