using CodeBase.BoardContent;
using CodeBase.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBase.UI {
  public class HUD : UIBehaviour {
    [SerializeField]
    private Button _buildTower;
    [SerializeField]
    private Button _buildGround;
    private TileContentBuilder _tileContentBuilder;
    public void Construct(TileContentBuilder contentBuilder) => _tileContentBuilder = contentBuilder;

    protected override void Start() {
      _buildTower.AddListener(BuildTower);
      _buildGround.AddListener(BuildGround);
      _tileContentBuilder.RunEvents();
    }

    protected override void OnDestroy() {
      _buildTower.RemoveListener(BuildTower);
      _buildGround.RemoveListener(BuildGround);
      _tileContentBuilder.StopEvents();
    }

    private void BuildGround() {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(TileContentType.Ground);
    }

    private void BuildTower() {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(TowerType.LTower);
    }
  }
}