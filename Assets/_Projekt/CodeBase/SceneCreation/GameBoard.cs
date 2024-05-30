using UnityEngine;

namespace CodeBase.SceneCreation {
  public class GameBoard : MonoBehaviour {
    [SerializeField]
    private Transform _ground;
    [SerializeField]
    private BoardTile _tilePrefab;
    
    private Vector2Int _boardSize;

    private BoardTile[] _tiles;

    public void Initialize(Vector2Int size) {
      _boardSize = size;
      _ground.localScale = new Vector3(size.x, size.y, 1.0f);
      Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);

      _tiles = new BoardTile[size.x * size.y];
      for (int i = 0, y = 0; y < size.y; y++) {
        for (int x = 0; x < size.x; x++, i++) {
          BoardTile tile = _tiles[i] = Instantiate(_tilePrefab, transform, false);
          tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
        }
      }
    }
  }
}