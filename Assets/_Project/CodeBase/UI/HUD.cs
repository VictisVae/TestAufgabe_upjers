using CodeBase.BoardContent;
using CodeBase.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBase.UI {
  public class HUD : UIBehaviour {
    [SerializeField]
    private Button _buildGround;
    [SerializeField]
    private Button _buildSpawnPoint;
    [SerializeField]
    private Button _buildDestination;
    [SerializeField]
    private Button _buildSimpleTower;
    [SerializeField]
    private Button _buildDoubleTower;
    [SerializeField]
    private Button _buildQuadTower;
    [SerializeField]
    private Button _buildLTower;
    [SerializeField]
    private Button _buildPlusTower;
    [SerializeField]
    private Button _buildUTower;
    private TileContentBuilder _tileContentBuilder;
    public void Construct(TileContentBuilder contentBuilder) => _tileContentBuilder = contentBuilder;

    protected override void Start() {
      _buildGround.AddListener(BuildGround);
      _buildSpawnPoint.AddListener(BuildSpawnPoint);
      _buildDestination.AddListener(BuildDestinationPoint);
      _buildSimpleTower.AddListener(BuildSimpleTower);
      _buildDoubleTower.AddListener(BuildDoubleTower);
      _buildQuadTower.AddListener(BuildQuadTower);
      _buildLTower.AddListener(BuildLTowerTower);
      _buildPlusTower.AddListener(BuildPlusTowerTower);
      _buildUTower.AddListener(BuildUTowerTower);
      _tileContentBuilder.RunEvents();
    }

    protected override void OnDestroy() {
      _buildGround.RemoveListener(BuildGround);
      _buildSpawnPoint.RemoveListener(BuildSpawnPoint);
      _buildDestination.RemoveListener(BuildDestinationPoint);
      _buildSimpleTower.RemoveListener(BuildSimpleTower);
      _buildDoubleTower.RemoveListener(BuildDoubleTower);
      _buildQuadTower.RemoveListener(BuildQuadTower);
      _buildLTower.RemoveListener(BuildLTowerTower);
      _buildPlusTower.RemoveListener(BuildPlusTowerTower);
      _buildUTower.RemoveListener(BuildUTowerTower);
      _tileContentBuilder.StopEvents();
    }

    private void BuildGround() {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(TileContentType.Ground);
    }
    
    private void BuildSpawnPoint() {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(TileContentType.SpawnPoint);
    }
    
    private void BuildDestinationPoint() {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(TileContentType.Destination);
    }

    private void BuildSimpleTower() {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(TowerType.Simple);
    }
    
    private void BuildDoubleTower() {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(TowerType.Double);
    }

    private void BuildQuadTower() {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(TowerType.Quad);
    }

    private void BuildLTowerTower() {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(TowerType.LTower);
    }

    private void BuildPlusTowerTower() {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(TowerType.PlusTower);
    }

    private void BuildUTowerTower() {
      _tileContentBuilder.AllowFlyingBuilding();
      _tileContentBuilder.StartPlacingContent(TowerType.UTower);
    }
  }
}