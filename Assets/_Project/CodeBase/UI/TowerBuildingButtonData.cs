using CodeBase.BoardContent;
using UnityEngine;

namespace CodeBase.UI {
  public class TowerBuildingButtonData : BuildingButtonBase {
    [field: SerializeField]
    public TowerType Type { get; private set; }
    protected override void Awake() => MeshButtonName.text = $"Build{Type}";
  }
}