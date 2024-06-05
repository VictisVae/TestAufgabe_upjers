using System;
using CodeBase.Infrastructure.Services;
using CodeBase.Units;
using UnityEngine;

namespace CodeBase.Infrastructure.Gameplay {
  [Serializable]
  public class EnemySpawnSequence {
    [SerializeField]
    private UnitType _type;
    [SerializeField]
    private int _amount = 1;
    [SerializeField]
    [Range(0.1f, 10.0f)]
    private float _cooldown = 1.0f;
    public State Begin() => new State(this);

    [Serializable]
    public struct State {
      private EnemySpawnSequence _sequence;
      private IUnitSpawner _unitSpawner;
      private int _count;
      private float _cooldown;

      public State(EnemySpawnSequence sequence) {
        _unitSpawner = GlobalService.Container.GetSingle<IUnitSpawner>();
        _sequence = sequence;
        _count = 0;
        _cooldown = _sequence._cooldown;
      }

      public float Progress(float deltaTime) {
        _cooldown += deltaTime;

        while (_cooldown >= _sequence._cooldown) {
          _cooldown -= _sequence._cooldown;

          if (_count >= _sequence._amount) {
            return _cooldown;
          }

          _count++;
          _unitSpawner.SpawnUnit(_sequence._type);
        }

        return -1.0f;
      }
    }
  }
}