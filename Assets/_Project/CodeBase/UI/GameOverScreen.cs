using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.Player;
using CodeBase.Infrastructure.States;
using CodeBase.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBase.UI {
  public class GameOverScreen : UIBehaviour {
    [SerializeField]
    private Button _restartButton;
    [SerializeField]
    private Button _quitButton;
    [SerializeField]
    private TextMeshProUGUI _gameOverText;
    [SerializeField]
    private Canvas _canvas;
    private IPlayerService _playerService;
    private IUnitSpawner _unitSpawner;
    private IGameStateMachine _gameStateMachine;
    private IGameFactory _gameFactory;
    private GameBoard _gameBoard;
    protected override void Awake() => _quitButton.AddListener(Application.Quit);

    protected override void OnDestroy() {
      _restartButton.RemoveAllListeners();
      _quitButton.RemoveAllListeners();
    }

    public void Construct(IPlayerService playerService, IUnitSpawner unitSpawner, GameBoard gameBoard) {
      _gameStateMachine = GlobalService.Container.GetSingle<IGameStateMachine>();
      _gameFactory = GlobalService.Container.GetSingle<IGameFactory>();
      _playerService = playerService;
      _gameBoard = gameBoard;
      _unitSpawner = unitSpawner;
      _restartButton.AddListener(Restart);
      _canvas.enabled = false;
    }

    public void Appear(bool isVictory) {
      _canvas.enabled = true;
      _gameOverText.text = isVictory ? "Victory!" : "Defeat";
    }

    private async void Restart() {
      await _gameFactory.Clear();
      _playerService.ResetValues();
      _unitSpawner.Collection.Clear();
      _gameBoard.Clear();
      _canvas.enabled = false;
      _gameStateMachine.Enter<BootstrapState>();
    }
  }
}