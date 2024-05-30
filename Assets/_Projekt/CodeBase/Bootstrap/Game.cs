using CodeBase.SceneCreation;
using UnityEngine;

namespace CodeBase.Bootstrap {
  public class Game : MonoBehaviour {
    [SerializeField]
    private GameBoard _board;

    [SerializeField]
    private Vector2Int _size;

    private void Start() => _board.Initialize(_size);
  }
}