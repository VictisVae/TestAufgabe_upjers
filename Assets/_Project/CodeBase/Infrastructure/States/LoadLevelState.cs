﻿using System.Collections.Generic;
using CodeBase.BoardContent;
using CodeBase.Grid;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.GameBoot;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services.Input;
using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Infrastructure.Services.Player;
using CodeBase.Infrastructure.Services.Random;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.UI;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Infrastructure.States {
  public class LoadLevelState : IPayloadState<string> {
    private readonly IMonoEventsProvider _monoEventsProvider;
    private readonly IGameFactory _gameFactory;
    private readonly IInputService _input;
    private readonly SceneLoader _sceneLoader;
    private readonly IStaticDataService _staticDataService;
    private readonly IUnitSpawner _spawner;
    private readonly IPlayerService _playerService;
    private readonly IRandomService _randomService;
    private List<MonoBehaviour> _clearData;

    private bool _alreadyWelcomed;

    public LoadLevelState
      (IInputService input, IMonoEventsProvider monoEventsProvider, IGameFactory gameFactory, IStaticDataService staticDataService,
        IUnitSpawner spawner, IPlayerService playerService, IRandomService randomService, SceneLoader sceneLoader) {
      _input = input;
      _sceneLoader = sceneLoader;
      _monoEventsProvider = monoEventsProvider;
      _gameFactory = gameFactory;
      _staticDataService = staticDataService;
      _playerService = playerService;
      _randomService = randomService;
      _spawner = spawner;
    }

    public void Enter(string sceneName) =>
      _sceneLoader.Load(sceneName, OnLoaded);

    public void Exit() {
      foreach (var data in _clearData) {
        Object.Destroy(data.gameObject);
      }
    }

    private void OnLoaded() {
      if (_alreadyWelcomed == false) {
        _alreadyWelcomed = true;
        _gameFactory.CreateWelcomeScreen();
      }

      _gameFactory.WarmUp();
      _clearData = new List<MonoBehaviour>();
      GridController gridController = new GridController(_gameFactory, _staticDataService, _playerService, _randomService);
      HUD hud = _gameFactory.CreateHUD();
      GameOverScreen gameOverScreen = _gameFactory.CreateGameOverScreen().With(x => x.Construct(_playerService, _spawner, gridController));

      ScenarioRunner scenarioRunner = ScenarioRunner(gridController, hud, gameOverScreen);
      
      TileContentBuilder tileContentBuilder
        = new TileContentBuilder(_gameFactory, _input, _spawner, _staticDataService, _monoEventsProvider, gridController);
      
      hud.Construct(_playerService, _staticDataService, tileContentBuilder, scenarioRunner);
      _spawner.Construct(gridController);
      gridController.InitialiseGridSystem();
      gridController.InitialiseGridView();
      gridController.Clear();
      _clearData.Add(hud);
      _clearData.Add(gameOverScreen);
    }
    
    private ScenarioRunner ScenarioRunner(GridController gridController, HUD hud, GameOverScreen gameOverScreen) =>
      new ScenarioRunner(_spawner, _monoEventsProvider, _staticDataService, _playerService, gridController, hud, gameOverScreen);
  }
}