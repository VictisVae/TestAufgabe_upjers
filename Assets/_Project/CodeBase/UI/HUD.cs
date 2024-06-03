using CodeBase.BoardContent;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeBase.UI {
  public class HUD : UIBehaviour {
    [SerializeField]
    private TileBuildingButtonData[] _tileBuildingButtons;
    [SerializeField]
    private TowerBuildingButtonData[] _towerBuildingButtons;
    [SerializeField]
    private WaveRunnerButton _waveRunner; 
    
    private TileContentBuilder _tileContentBuilder;
    private Player _player;

    protected override void Start() {
      foreach (TileBuildingButtonData tileBuildingButton in _tileBuildingButtons) {
        tileBuildingButton.Button.AddListener(() => BuildTile(tileBuildingButton.Type));
      }

      foreach (TowerBuildingButtonData towerBuildingButton in _towerBuildingButtons) {
        towerBuildingButton.Button.AddListener(() => BuildTower(towerBuildingButton.Type));
      }

      _waveRunner.SubscribeAction(RunFirstWave);
      _tileContentBuilder.RunEvents();
    }

    protected override void OnDestroy() {
      foreach (TileBuildingButtonData tileBuildingButton in _tileBuildingButtons) {
        tileBuildingButton.Button.RemoveAllListeners();
      }

      foreach (TowerBuildingButtonData towerBuildingButton in _towerBuildingButtons) {
        towerBuildingButton.Button.RemoveAllListeners();
      }

      _waveRunner.UnsubscribeAction(RunFirstWave);
      _tileContentBuilder.StopEvents();
    }

    public void Construct(TileContentBuilder contentBuilder, Player player) {
      _player = player;
      _tileContentBuilder = contentBuilder;
    }

    private void RunFirstWave() {
      _player.BeginNewGame();
      _waveRunner.SetDisabled();
    }

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