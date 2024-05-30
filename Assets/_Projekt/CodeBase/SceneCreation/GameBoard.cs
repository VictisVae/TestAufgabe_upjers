using UnityEngine;

namespace CodeBase.SceneCreation {
  public class GameBoard : MonoBehaviour {
    [SerializeField]
    private Transform _ground;
    private Vector2Int _boardSize;

    public void Initialize(Vector2Int size) {
      _boardSize = size;
      _ground.localScale = new Vector3(size.x, size.y, 1.0f);
    }
  }
}