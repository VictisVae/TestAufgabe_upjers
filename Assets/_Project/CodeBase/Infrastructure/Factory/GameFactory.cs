using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services.AssetManagement;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.SceneCreation;
using CodeBase.Units;
using CodeBase.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Infrastructure.Factory {
  public class GameFactory : IGameFactory {
    private readonly IAsset _asset;
    private readonly IStaticDataService _staticDataService;
    private Scene _scene;

    public GameFactory(IAsset asset, IStaticDataService staticDataService) {
      _asset = asset;
      _staticDataService = staticDataService;
    }

    public GameBoard CreateGameBoard() => _asset.Initialize<GameBoard>(Constants.AssetsPath.GameBoard);

    public TileContent Create(TileContentType type) {
      TileContent instance = CreateGameObjectInstance(_staticDataService.GetStaticData<TileContentStorage>().GetTileContent(type));
      instance.Construct(this);
      return instance;
    }

    public UnitBase Create(UnitType type) {
      UnitConfig config = _staticDataService.GetStaticData<UnitConfigStorage>().GetUnitConfig(type);
      UnitBase instance = CreateGameObjectInstance(config.Prefab);
      instance.Construct(this, config);
      return instance;
    }

    public void Reclaim(FactoryObject unit) => Object.Destroy(unit.gameObject);

    private T CreateGameObjectInstance<T>(T prefab) where T : MonoBehaviour {
      if (_scene.isLoaded == false) {
        if (Application.isEditor) {
          _scene = SceneManager.GetSceneByName(nameof(GameFactory));

          if (_scene.isLoaded == false) {
            _scene = SceneManager.CreateScene(nameof(GameFactory));
          }
        } else {
          _scene = SceneManager.CreateScene(nameof(GameFactory));
        }
      }

      T instance = Object.Instantiate(prefab);
      SceneManager.MoveGameObjectToScene(instance.gameObject, _scene);
      return instance;
    }
  }
}