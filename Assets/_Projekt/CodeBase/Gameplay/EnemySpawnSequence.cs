using System;
using CodeBase.Bootstrap;
using CodeBase.Units;
using UnityEngine;

namespace CodeBase.Gameplay {
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
      private int _count;
      private float _cooldown;

      public State(EnemySpawnSequence sequence) {
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
          Game.SpawnUnit(_sequence._type);
        }

        return -1.0f;
      }
    }
  }
}