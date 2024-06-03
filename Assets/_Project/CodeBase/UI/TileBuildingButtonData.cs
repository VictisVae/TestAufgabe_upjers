using CodeBase.BoardContent;
using UnityEngine;

namespace CodeBase.UI {
  public class TileBuildingButtonData : BuildingButtonBase {
    [field: SerializeField]
    public TileContentType Type { get; private set; }
    protected override void Awake() => TextMesh.text = $"Build{Type}";
  }
}