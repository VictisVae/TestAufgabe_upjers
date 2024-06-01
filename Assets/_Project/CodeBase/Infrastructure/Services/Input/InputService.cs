using UnityEngine;

namespace CodeBase.Infrastructure.Services.Input {
  public class InputService : IInputService {
    public bool KeyDown(KeyCode key) => UnityEngine.Input.GetKeyDown(key);
    public bool Key(KeyCode key) => UnityEngine.Input.GetKey(key);
    public bool MouseButtonDown(int buttonIndex) => UnityEngine.Input.GetMouseButtonDown(buttonIndex);
    public Vector3 MousePosition => UnityEngine.Input.mousePosition;
  }
}