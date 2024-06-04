using CodeBase.BoardContent;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Pool;
using CodeBase.Infrastructure.Services.AssetManagement;
using CodeBase.Infrastructure.Services.Player;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Infrastructure.Services.StaticData.TileContentData;
using CodeBase.Infrastructure.Services.StaticData.TowerData;
using CodeBase.Infrastructure.Services.StaticData.UnitData;
using CodeBase.UI;
using CodeBase.Units;
using CodeBase.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Infrastructure.Factory {
  public class GameFactory : IGameFactory {
    private readonly IAsset _asset;
    private readonly IStaticDataService _staticDataService;
    private readonly IPlayerService _playerService;
    private readonly TurretBulletPool _turretBulletPool;
    private Scene _scene;

    public GameFactory(IAsset asset, IStaticDataService staticDataService, IPlayerService playerService) {
      _asset = asset;
      _staticDataService = staticDataService;
      _playerService = playerService;
      _turretBulletPool = new TurretBulletPool(InitBullet);
    }

    public GameBoard CreateGameBoard() => _asset.Initialize<GameBoard>(Constants.AssetsPath.GameBoard);
    public HUD CreateHUD() => _asset.Initialize<HUD>(Constants.AssetsPath.HUD);

    public Tower CreateTower(TowerType towerType) {
      TowerConfig towerConfig = _staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(towerType);
      Tower instance = CreateGameObjectInstance(towerConfig.Prefab);
      instance.Construct(towerConfig);
      instance.Construct(this);
      return instance;
    }

    public TurretBullet CreateBullet() => _turretBulletPool.Get();
    
    public TileContent Create(TileContentType type) {
      TileContent instance = CreateGameObjectInstance(_staticDataService.GetStaticData<TileContentStorage>().GetTileContent(type));
      instance.Construct(this);
      return instance;
    }

    public UnitBase Create(UnitType type) {
      UnitConfig config = _staticDataService.GetStaticData<UnitConfigStorage>().GetUnitConfig(type);
      UnitBase instance = CreateGameObjectInstance(config.Prefab);
      instance.Construct(this, _playerService, config);
      return instance;
    }

    public void Reclaim(FactoryObject unit) => Object.Destroy(unit.gameObject);
    public void ReclaimBullet(TurretBullet bullet) => _turretBulletPool.Return(bullet);

    private T CreateGameObjectInstance<T>(T prefab) where T : MonoBehaviour {
      string sceneName = prefab.GetType().Name;

      if (Application.isEditor) {
        _scene = SceneManager.GetSceneByName(sceneName);

        if (_scene.isLoaded == false) {
          _scene = SceneManager.CreateScene(sceneName);
        }
      } else {
        if (_scene.isLoaded == false) {
          _scene = SceneManager.CreateScene(sceneName);
        }
      }

      T instance = Object.Instantiate(prefab);
      SceneManager.MoveGameObjectToScene(instance.gameObject, _scene);
      return instance;
    }

    private TurretBullet InitBullet() {
      TurretBulletData turretBulletData = _staticDataService.GetStaticData<TurretBulletData>();
      return CreateGameObjectInstance(turretBulletData.Prefab).With(x => x.SetBulletSpeed(turretBulletData.Speed));
    }
  }
}