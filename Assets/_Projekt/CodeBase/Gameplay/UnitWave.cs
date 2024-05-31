using System;
using UnityEngine;

namespace CodeBase.Gameplay {
  [CreateAssetMenu]
  public class UnitWave : ScriptableObject {
    [SerializeField]
    private EnemySpawnSequence[] _spawnSequences;

    public State Begin() => new State(this);
    
    [Serializable]
    public struct State {
      private UnitWave _wave;
      private int _index;
      private EnemySpawnSequence.State _sequence;

      public State(UnitWave wave) {
        _wave = wave;
        _index = 0;
        _sequence = _wave._spawnSequences[0].Begin();
      }
      
      public float Progress(float deltaTime) {
        deltaTime = _sequence.Progress(deltaTime);
        
        while (deltaTime >= 0) {
          if (++_index >= _wave._spawnSequences.Length) {
            return deltaTime;
          }

          _sequence = _wave._spawnSequences[_index].Begin();
          deltaTime = _sequence.Progress(deltaTime);
        }

        return -1.0f;
      }
    }
  }
}