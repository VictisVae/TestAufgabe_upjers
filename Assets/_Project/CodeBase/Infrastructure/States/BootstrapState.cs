using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.GameBoot;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.AssetManagement;
using CodeBase.Infrastructure.Services.Input;
using CodeBase.Infrastructure.Services.Player;
using CodeBase.Infrastructure.Services.Random;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.Utilities;

namespace CodeBase.Infrastructure.States {
  public class BootstrapState : IState {
    private readonly IGameStateMachine _stateMachine;
    private readonly SceneLoader _sceneLoader;
    private readonly GlobalService _globalService;

    public BootstrapState(IGameStateMachine stateMachine, SceneLoader sceneLoader, GlobalService globalService) {
      _stateMachine = stateMachine;
      _sceneLoader = sceneLoader;
      _globalService = globalService;
      RegisterServices();
    }

    public void Enter() => _sceneLoader.Load(Constants.Boot.Bootstrap, EnterLoadLevel);
    public void Exit() {}

    private void RegisterServices() {
      _globalService.RegisterSingle<IStaticDataService>(new StaticDataService().With(x => x.Initialize()));
      _globalService.RegisterSingle(_stateMachine);
      _globalService.RegisterSingle<IAsset>(new AssetProvider());
      _globalService.RegisterSingle<IPlayerService>(new PlayerService(_globalService.GetSingle<IStaticDataService>()));

      _globalService.RegisterSingle<IGameFactory>(new GameFactory(_globalService.GetSingle<IAsset>(), _globalService.GetSingle<IStaticDataService>(),
        _globalService.GetSingle<IPlayerService>()));

      _globalService.RegisterSingle<IInputService>(new InputService());
      _globalService.RegisterSingle<IRandomService>(new RandomService());

      _globalService.RegisterSingle<IUnitSpawner>(new UnitSpawner(_globalService.GetSingle<IRandomService>(),
        _globalService.GetSingle<IGameFactory>()));
    }

    private void EnterLoadLevel() => _stateMachine.Enter<LoadLevelState, string>(Constants.Boot.Main);
  }
}