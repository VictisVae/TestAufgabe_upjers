using CodeBase.BoardContent;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.GameBoot;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services.Input;
using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Infrastructure.Services.StaticData;

namespace CodeBase.Infrastructure.States {
  public class LoadLevelState : IPayloadState<string> {
    private readonly IMonoEventsProvider _monoEventsProvider;
    private readonly IGameFactory _gameFactory;
    private readonly IInputService _input;
    private readonly SceneLoader _sceneLoader;
    private readonly IStaticDataService _staticDataService;
    private readonly IUnitSpawner _spawner;
    private readonly ICoroutineHandler _coroutineHandler;
    private GameplayModerator _moderator;

    public LoadLevelState
      (IInputService input, IMonoEventsProvider monoEventsProvider, IGameFactory gameFactory, IStaticDataService staticDataService,
        IUnitSpawner spawner, SceneLoader sceneLoader) {
      _input = input;
      _sceneLoader = sceneLoader;
      _monoEventsProvider = monoEventsProvider;
      _gameFactory = gameFactory;
      _staticDataService = staticDataService;
      _spawner = spawner;
      _coroutineHandler = sceneLoader.CoroutineHandler;
    }

    public void Enter(string sceneName) =>
      _sceneLoader.Load(sceneName, OnLoaded);

    public void Exit() => _moderator?.StopEvents();

    private void OnLoaded() {
      var hud = _gameFactory.CreateHUD();
      GameBoard gameBoard = CreateGameBoard();
      hud.Construct(new TileContentBuilder(_gameFactory, _input, _spawner, _staticDataService, _monoEventsProvider, gameBoard));
      _moderator = CreateGameplayModerator(gameBoard);
      _spawner.Construct(gameBoard);
      gameBoard.Initialize(_staticDataService, _gameFactory);
      _moderator.RunEvents();
      _moderator.BeginNewGame();
    }

    private GameBoard CreateGameBoard() => _gameFactory.CreateGameBoard();

    private GameplayModerator CreateGameplayModerator(GameBoard gameBoard) => new GameplayModerator(_spawner, _input, _monoEventsProvider, _coroutineHandler, _staticDataService, gameBoard);
  }
}