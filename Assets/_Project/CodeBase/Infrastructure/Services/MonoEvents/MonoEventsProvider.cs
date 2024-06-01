using System;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.MonoEvents {
  public class MonoEventsProvider : MonoBehaviour, IMonoEventsProvider {
    public event Action OnApplicationUpdateEvent = delegate {};
    private void Awake() => DontDestroyOnLoad(this);
    private void Update() => OnApplicationUpdateEvent();
  }
}