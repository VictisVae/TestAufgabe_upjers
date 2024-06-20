using UnityEngine;

namespace CodeBase.Infrastructure.Services.Input {
  public class InputService : IInputService {
    public bool KeyDown(KeyCode key) => UnityEngine.Input.GetKeyDown(key);
    public bool Key(KeyCode key) => UnityEngine.Input.GetKey(key);
    public bool MouseButtonDown(int buttonIndex) => UnityEngine.Input.GetMouseButtonDown(buttonIndex);
    public bool HasPosition(out RaycastHit hit) => Physics.Raycast(TouchRay, out hit, float.MaxValue, 1);
    public Vector3 MouseWorldPosition {
      get {
        Physics.Raycast(TouchRay, out RaycastHit hit, float.MaxValue, 1);
        return hit.point;
      }
    }
    private Vector3 MousePosition => UnityEngine.Input.mousePosition;
    private Ray TouchRay => Camera.main.ScreenPointToRay(MousePosition);
  }
}