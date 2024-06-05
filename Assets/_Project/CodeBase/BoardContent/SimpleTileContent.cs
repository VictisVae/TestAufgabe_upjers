using UnityEngine;

namespace CodeBase.BoardContent {
  public class SimpleTileContent : TileContent {
    [SerializeField]
    private TileRecolor _tileRecolor;

    private void Start() => _tileRecolor.SetTileType(_type);
    public override void ViewAvailable(bool isAvailable) => _tileRecolor.ViewAvailable(isAvailable);
    public override void SetNormal() => _tileRecolor.SetNormal();
  }
}