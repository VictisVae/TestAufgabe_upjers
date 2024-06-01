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
      private int _index;
      private UnitWave.State _wave;

      public State(GameScenario scenario) {
        _scenario = scenario;
        _index = 0;
        _wave = _scenario._waves[0].Begin();
      }

      public bool Progress() {
        float deltaTime = _wave.Progress(Time.deltaTime);

        while (deltaTime >= 0) {
          if (++_index >= _scenario._waves.Length) {
            return false;
          }

          _wave = _scenario._waves[_index].Begin();
          deltaTime = _wave.Progress(deltaTime);
        }

        return true;
      }
    }
  }
}