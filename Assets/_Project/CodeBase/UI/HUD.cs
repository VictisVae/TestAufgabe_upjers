using CodeBase.BoardContent;
using CodeBase.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeBase.UI {
  public class HUD : UIBehaviour {
    [SerializeField]
    private TileBuildingButtonData[] _tileBuildingButtons;
    [SerializeField]
    private TowerBuildingButtonData[] _towerBuildingButtons;
    private TileContentBuilder _tileContentBuilder;

    protected override void Start() {
      foreach (TileBuildingButtonData tileBuildingButton in _tileBuildingButtons) {
        tileBuildingButton.Button.AddListener(() => BuildTile(tileBuildingButton.Type));
      }

      foreach (TowerBuildingButtonData towerBuildingButton in _towerBuildingButtons) {
        towerBuildingButton.Button.AddListener(() => BuildTower(towerBuildingButton.Type));
      }

      _tileContentBuilder.RunEvents();
    }

    protected override void OnDestroy() {
      foreach (TileBuildingButtonData tileBuildingButton in _tileBuildingButtons) {
        tileBuildingButton.Button.RemoveAllListeners();
      }

      foreach (TowerBuildingButtonData towerBuildingButton in _towerBuildingButtons) {
        towerBuildingButton.Button.RemoveAllListeners();
      }

      _tileContentBuilder.StopEvents();
    }

    public void Construct(TileContentBuilder contentBuilder) => _tileContentBuilder = contentBuilder;

    private void BuildTile(TileContentType tileContentType) {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(tileContentType);
    }

    private void BuildTower(TowerType towerType) {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(towerType);
    }
  }
}