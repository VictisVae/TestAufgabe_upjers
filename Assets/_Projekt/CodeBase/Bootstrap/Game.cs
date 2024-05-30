using CodeBase.SceneCreation;
using UnityEngine;

namespace CodeBase.Bootstrap {
  public class Game : MonoBehaviour {
    [SerializeField]
    private GameBoard _board;
    [SerializeField]
    private Vector2Int _size;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private TileContentFactory _contentFactory;
    private void Start() => _board.Initialize(_size, _contentFactory);

    private void Update() {
      if (Input.GetMouseButtonDown(0)) {
        HandleTouch();
      } else if (Input.GetMouseButtonDown(1)) {
        HandleAlternativeTouch();
      }
    }

    private void HandleTouch() {
      BoardTile tile = _board.GetTile(TouchRay);

      if (tile != null) {
        _board.ToggleGround(tile);
      }
    }

    private void HandleAlternativeTouch() {
      BoardTile tile = _board.GetTile(TouchRay);

      if (tile != null) {
        _board.ToggleDestination(tile);
      }
    }

    private Ray TouchRay => _camera.ScreenPointToRay(Input.mousePosition);
  }
}