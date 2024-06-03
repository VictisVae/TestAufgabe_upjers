using System;
using CodeBase.Infrastructure.Gameplay;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.StaticData {
  [CreateAssetMenu]
  public class GameScenario : EntityStaticData {
    [SerializeField]
    private UnitWave[] _waves;
    public State Begin() => new State(this);

    [Serializable]
    public struct State {
      private GameScenario _scenario;
      private UnitWave.State _wave;
      private int _index;

      public State(GameScenario scenario) {
        _scenario = scenario;
        _index = 0;
        _wave = _scenario._waves[0].Begin();
        NextWaveReady = false;
      }

      public bool Progress() {
        float deltaTime = _wave.Progress(Time.deltaTime);

        while (deltaTime >= 0) {
          if (_wave.WaveCompleted) {
            NextWaveReady = true;
            return false;
          }

          NextWaveReady = false;
          deltaTime = _wave.Progress(deltaTime);
        }

        return true;
      }

      public bool NextWaveReady { get; private set; }

      public bool NextWave() {
        if (_index + 1 >= _scenario._waves.Length) {
          return false;
        }

        _wave = _scenario._waves[++_index].Begin();
        return true;
      }
    }
  }
}