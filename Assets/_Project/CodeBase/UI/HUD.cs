using CodeBase.BoardContent;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services.Player;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Infrastructure.Services.StaticData.TileContentData;
using CodeBase.Infrastructure.Services.StaticData.TowerData;
using CodeBase.Towers;
using CodeBase.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CodeBase.UI {
  public class HUD : UIBehaviour {
    [SerializeField]
    private TileBuildingButtonData[] _tileBuildingButtons;
    [SerializeField]
    private TowerBuildingButtonData[] _towerBuildingButtons;
    [SerializeField]
    private WaveRunnerButton _waveRunner;
    [SerializeField]
    private PlayerView _playerView;
    private TileContentBuilder _tileContentBuilder;
    private ScenarioRunner _scenarioRunner;
    private IPlayerService _playerService;
    private IStaticDataService _staticDataService;

    protected override void Start() {
      var tileStorage = _staticDataService.GetStaticData<TileContentStorage>();

      foreach (TileBuildingButtonData tileBuildingButton in _tileBuildingButtons) {
        var config = tileStorage.GetTileConfig(tileBuildingButton.Type);
        tileBuildingButton.SetBuildValue(config.GoldValue, config.BringsGold, _playerService.Gold);
        tileBuildingButton.SetLimit(config.IsLimited, config.Limit);
        tileBuildingButton.Button.AddListener(() => BuildTile(tileBuildingButton.Type));
        _playerService.OnGoldChangedEvent += tileBuildingButton.UpdateButtonAvailability;
      }

      var towerStorage = _staticDataService.GetStaticData<TowerContentStorage>();

      foreach (TowerBuildingButtonData towerBuildingButton in _towerBuildingButtons) {
        var config = towerStorage.GetTowerConfig(towerBuildingButton.Type);
        towerBuildingButton.SetBuildValue(config.GoldValue, false, _playerService.Gold);
        towerBuildingButton.Button.AddListener(() => BuildTower(towerBuildingButton.Type));
        _playerService.OnGoldChangedEvent += towerBuildingButton.UpdateButtonAvailability;
      }

      _playerView.SetCurrentHealth(_playerService.Health);
      _playerView.SetCurrentGold(_playerService.Gold);
      _playerService.OnHealthChangedEvent += _playerView.SetCurrentHealth;
      _playerService.OnGoldChangedEvent += _playerView.SetCurrentGold;
      _waveRunner.UpdateDisplay(_scenarioRunner.TotalWaves);
      _waveRunner.SubscribeAction(RunFirstWave);
      _tileContentBuilder.RunEvents();
    }

    protected override void OnDestroy() {
      foreach (TileBuildingButtonData tileBuildingButton in _tileBuildingButtons) {
        _playerService.OnGoldChangedEvent -= tileBuildingButton.UpdateButtonAvailability;
        tileBuildingButton.Button.RemoveAllListeners();
      }

      foreach (TowerBuildingButtonData towerBuildingButton in _towerBuildingButtons) {
        _playerService.OnGoldChangedEvent -= towerBuildingButton.UpdateButtonAvailability;
        towerBuildingButton.Button.RemoveAllListeners();
      }

      _playerService.OnHealthChangedEvent -= _playerView.SetCurrentHealth;
      _playerService.OnGoldChangedEvent -= _playerView.SetCurrentGold;
      _waveRunner.UnsubscribeAll();
      _tileContentBuilder.StopEvents();
    }

    public void Construct(IPlayerService playerService, IStaticDataService staticDataService, TileContentBuilder contentBuilder, ScenarioRunner scenarioRunner) {
      _staticDataService = staticDataService;
      _playerService = playerService;
      _scenarioRunner = scenarioRunner;
      _tileContentBuilder = contentBuilder;
    }

    private void RunFirstWave() => _scenarioRunner.BeginNewGame();

    private void BuildTile(TileContentType tileContentType) {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(tileContentType);
    }

    private void BuildTower(TowerType towerType) {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(towerType);
    }

    public void SetNextWaveReady(UnityAction runNextWave) {
      _waveRunner.UnsubscribeAction(RunFirstWave);
      _waveRunner.UnsubscribeAction(runNextWave);
      _waveRunner.SetEnabled();
      _waveRunner.SubscribeAction(runNextWave);
    }
  }
}