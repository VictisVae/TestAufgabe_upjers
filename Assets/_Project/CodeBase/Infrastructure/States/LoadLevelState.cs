using CodeBase.BoardContent;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.GameBoot;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services.Input;
using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.UI;

namespace CodeBase.Infrastructure.States {
  public class LoadLevelState : IPayloadState<string> {
    private readonly IMonoEventsProvider _monoEventsProvider;
    private readonly IGameFactory _gameFactory;
    private readonly IInputService _input;
    private readonly SceneLoader _sceneLoader;
    private readonly IStaticDataService _staticDataService;
    private readonly IUnitSpawner _spawner;

    public LoadLevelState
      (IInputService input, IMonoEventsProvider monoEventsProvider, IGameFactory gameFactory, IStaticDataService staticDataService,
        IUnitSpawner spawner, SceneLoader sceneLoader) {
      _input = input;
      _sceneLoader = sceneLoader;
      _monoEventsProvider = monoEventsProvider;
      _gameFactory = gameFactory;
      _staticDataService = staticDataService;
      _spawner = spawner;
    }

    public void Enter(string sceneName) =>
      _sceneLoader.Load(sceneName, OnLoaded);

    public void Exit() {}

    private void OnLoaded() {
      HUD hud = _gameFactory.CreateHUD();
      GameBoard gameBoard = CreateGameBoard();
      Player player = CreatePlayer(gameBoard, hud);

      TileContentBuilder tileContentBuilder
        = new TileContentBuilder(_gameFactory, _input, _spawner, _staticDataService, _monoEventsProvider, gameBoard);

      hud.Construct(tileContentBuilder, player);
      _spawner.Construct(gameBoard);
      gameBoard.Initialize(_staticDataService, _gameFactory);
    }

    private GameBoard CreateGameBoard() => _gameFactory.CreateGameBoard();
    private Player CreatePlayer(GameBoard gameBoard, HUD hud) => new Player(_spawner, _monoEventsProvider, _staticDataService, gameBoard, hud);
  }
}