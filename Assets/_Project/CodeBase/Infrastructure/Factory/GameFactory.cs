using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBase.BoardContent;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Pool;
using CodeBase.Infrastructure.Services.AssetManagement;
using CodeBase.Infrastructure.Services.Player;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Infrastructure.Services.StaticData.TileContentData;
using CodeBase.Infrastructure.Services.StaticData.TowerData;
using CodeBase.Infrastructure.Services.StaticData.UnitData;
using CodeBase.Towers;
using CodeBase.UI;
using CodeBase.Units;
using CodeBase.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CodeBase.Infrastructure.Factory {
  public class GameFactory : IGameFactory {
    private readonly IAsset _asset;
    private readonly IStaticDataService _staticDataService;
    private readonly IPlayerService _playerService;
    private List<Scene> _loadedScenes;
    private Dictionary<TowerType, TowerPool> _towerPoolMap;
    private Dictionary<TileContentType, TilePool> _tilePoolMap;
    private Dictionary<UnitType, UnitPool> _unitPoolMap;
    private Scene _scene;

    public GameFactory(IAsset asset, IStaticDataService staticDataService, IPlayerService playerService) {
      _asset = asset;
      _staticDataService = staticDataService;
      _playerService = playerService;
    }

    public void WarmUp() {
      _loadedScenes = new List<Scene>();
      _towerPoolMap = new Dictionary<TowerType, TowerPool>();
      Array towerTypes = Enum.GetValues(typeof(TowerType));

      foreach (object type in towerTypes) {
        TowerType towerType = (TowerType)type;
        Tower towerPrefab = _staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(towerType).Prefab;
        _towerPoolMap.Add(towerType, new TowerPool(() => CreateGameObjectInstance(towerPrefab)));
      }

      _tilePoolMap = new Dictionary<TileContentType, TilePool>();
      Array tileTypes = Enum.GetValues(typeof(TileContentType));

      foreach (object type in tileTypes) {
        TileContentType tileType = (TileContentType)type;
        TileContent tileContentPrefab = _staticDataService.GetStaticData<TileContentStorage>().GetTileContent(tileType);
        _tilePoolMap.Add(tileType, new TilePool(() => CreateGameObjectInstance(tileContentPrefab)));
      }

      _unitPoolMap = new Dictionary<UnitType, UnitPool>();
      Array unitTypes = Enum.GetValues(typeof(UnitType));

      foreach (object type in unitTypes) {
        UnitType unitType = (UnitType)type;
        UnitBase unitPrefab = _staticDataService.GetStaticData<UnitConfigStorage>().GetUnitConfig(unitType).Prefab;
        _unitPoolMap.Add(unitType, new UnitPool(() => CreateGameObjectInstance(unitPrefab)));
      }
    }

    public GameBoard CreateGameBoard() => _asset.Initialize<GameBoard>(Constants.AssetsPath.GameBoard);
    public HUD CreateHUD() => _asset.Initialize<HUD>(Constants.AssetsPath.HUD);
    public GameOverScreen CreateGameOverScreen() => _asset.Initialize<GameOverScreen>(Constants.AssetsPath.GameOverScreen);
    public WelcomeScreen CreateWelcomeScreen() => _asset.Initialize<WelcomeScreen>(Constants.AssetsPath.WelcomeScreen);

    public Tower CreateTower(TowerType towerType) {
      TowerConfig towerConfig = _staticDataService.GetStaticData<TowerContentStorage>().GetTowerConfig(towerType);
      Tower instance = _towerPoolMap[towerType].Get();
      instance.Construct(towerConfig);
      instance.Construct(this);
      return instance;
    }

    public TileContent Create(TileContentType type) {
      TileContent instance = _tilePoolMap[type].Get();
      instance.Construct(this);
      return instance;
    }

    public UnitBase Create(UnitType type) {
      UnitConfig config = _staticDataService.GetStaticData<UnitConfigStorage>().GetUnitConfig(type);
      UnitBase instance = _unitPoolMap[type].Get();
      instance.Construct(this, _playerService, config);
      return instance;
    }

    public void Reclaim(FactoryObject item, Enum itemType) {
      switch (itemType) {
        case TowerType type:
          _towerPoolMap[type].Return((Tower)item);
          break;
        case TileContentType type:
          _tilePoolMap[type].Return((TileContent)item);
          break;
        case UnitType type:
          _unitPoolMap[type].Return((UnitBase)item);
          break;
      }
    }

    public async Task Clear() {
      foreach (Scene scene in _loadedScenes) {
        if (scene.isLoaded == false) {
          continue;
        }

        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(scene);

        while (unloadOp.isDone == false) {
          await Task.Yield();
        }
      }

      _towerPoolMap.Clear();
      _tilePoolMap.Clear();
      _unitPoolMap.Clear();
      _loadedScenes.Clear();
    }

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

      if (_loadedScenes.Contains(_scene) == false) {
        _loadedScenes.Add(_scene);
      }

      T instance = Object.Instantiate(prefab);
      SceneManager.MoveGameObjectToScene(instance.gameObject, _scene);
      return instance;
    }
  }
}