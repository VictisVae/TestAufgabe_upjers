﻿using System.Collections;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services.Input;
using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Infrastructure.Services.StaticData;
using CodeBase.SceneCreation;
using UnityEngine;

namespace CodeBase.Infrastructure.GameBoot {
  public class GameplayModerator {
    private readonly Camera _camera;
    private readonly IInputService _input;
    private readonly IMonoEventsProvider _monoEventsProvider;
    private readonly ICoroutineHandler _coroutineHandler;
    private readonly GameScenario _scenario;
    private readonly GameBoard _board;
    private readonly IUnitSpawner _unitSpawner;
    private readonly float _preparationTime = 10.0f;
    private readonly int _startingPlayerHealth = 100;
    private GameScenario.State _activeScenario;
    private int _currentPlayerHealth;
    private float _spawnProgress;
    private bool _scenarioInProgress;
    private bool _isPaused;
    private Coroutine _prepareRoutine;

    public GameplayModerator
      (IUnitSpawner unitSpawner, IInputService input, IMonoEventsProvider monoEventsProvider, ICoroutineHandler coroutineHandler,
        IStaticDataService staticDataService, GameBoard board) {
      _camera = Camera.main;
      _unitSpawner = unitSpawner;
      _board = board;
      _input = input;
      _monoEventsProvider = monoEventsProvider;
      _coroutineHandler = coroutineHandler;
      _scenario = staticDataService.GetStaticData<GameScenario>();
    }

    private void Update() {
      if (_input.KeyDown(KeyCode.Space)) {
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0.0f : 1.0f;
      }

      if (_input.KeyDown(KeyCode.R)) {
        BeginNewGame();
      }

      if (_input.MouseButtonDown(0)) {
        HandleTouch();
      } else if (_input.MouseButtonDown(1)) {
        HandleAlternativeTouch();
      }

      if (_scenarioInProgress) {
        if (_currentPlayerHealth <= 0) {
          Debug.Log("Defeated");
          BeginNewGame();
        }

        if (_activeScenario.Progress() == false && _unitSpawner.Collection.IsEmpty) {
          Debug.Log("Victory");
          BeginNewGame();
          _activeScenario.Progress();
          return;
        }
      }

      _unitSpawner.Collection.GameUpdate();
      _board.GameUpdate();
    }

    public void RunEvents() => _monoEventsProvider.OnApplicationUpdateEvent += Update;
    public void StopEvents() => _monoEventsProvider.OnApplicationUpdateEvent -= Update;

    private void HandleTouch() {
      BoardTile tile = _board.GetTile(TouchRay);

      if (tile == null) {
        return;
      }

      if (_input.Key(KeyCode.LeftShift)) {
        _board.ToggleTower(tile, () => _unitSpawner.Collection.Targets);
      } else {
        _board.ToggleGround(tile);
      }
    }

    private void HandleAlternativeTouch() {
      BoardTile tile = _board.GetTile(TouchRay);

      if (tile == null) {
        return;
      }

      if (_input.Key(KeyCode.LeftShift)) {
        _board.ToggleDestination(tile);
      } else {
        _board.ToggleSpawnPoint(tile);
      }
    }

    public void BeginNewGame() {
      _scenarioInProgress = false;

      if (_prepareRoutine != null) {
        _coroutineHandler.StopCoroutine(_prepareRoutine);
      }

      _unitSpawner.Collection.Clear();
      _board.Clear();
      _currentPlayerHealth = _startingPlayerHealth;
      _prepareRoutine = _coroutineHandler.StartCoroutine(PrepareRoutine());
    }

    private IEnumerator PrepareRoutine() {
      yield return new WaitForSeconds(_preparationTime);
      _activeScenario = _scenario.Begin();
      _scenarioInProgress = true;
    }

    private Ray TouchRay => _camera.ScreenPointToRay(_input.MousePosition);
  }
}