using UnityEngine;

namespace CodeBase.Infrastructure.Services.Input {
  public interface IInputService : IService {
    bool KeyDown(KeyCode key);
    bool Key(KeyCode key);
    bool MouseButtonDown(int buttonIndex);
    Vector3 MousePosition { get; }
  }
}