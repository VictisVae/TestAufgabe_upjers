using CodeBase.Data;
using CodeBase.SceneCreation;
using CodeBase.Units;
using UnityEngine;

namespace CodeBase.Factory {
  [CreateAssetMenu]
  public class UnitFactory : BaseFactory {
    [SerializeField]
    private GroundUnit _groundUnitPrefab;
    [SerializeField]
    private AirUnit _airUnitPrefab;
    [SerializeField]
    [FloatRangeSlider(0.2f, 5f)]
    private FloatRange _speed = new FloatRange(1.0f);
    [SerializeField]
    [FloatRangeSlider(0.5f, 2.0f)]
    private FloatRange _scale = new FloatRange(1.0f);
    [SerializeField]
    [FloatRangeSlider(-0.4f, 0.4f)]
    private FloatRange _pathOffset = new FloatRange(0.0f);
    public void Reclaim(UnitBase unit) => Destroy(unit.gameObject);

    public UnitBase Get() {
      UnitBase spawnPrefab = Random.Range(0, 2) == 0 ?  _groundUnitPrefab : _airUnitPrefab;
      UnitBase instance = CreateGameObjectInstance(spawnPrefab);
      instance.Initialise(this, _scale.RandomValueInRange, _pathOffset.RandomValueInRange, _speed.RandomValueInRange);
      return instance;
    }
  }
}