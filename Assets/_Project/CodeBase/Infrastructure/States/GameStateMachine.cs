using System;
using System.Collections.Generic;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.GameBoot;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.Input;
using CodeBase.Infrastructure.Services.MonoEvents;
using CodeBase.Infrastructure.Services.StaticData;

namespace CodeBase.Infrastructure.States {
  public class GameStateMachine : IGameStateMachine {
    private readonly Dictionary<Type, IExitState> _states;
    private IExitState _activeState;

    public GameStateMachine(SceneLoader sceneLoader, GlobalService globalService, IMonoEventsProvider monoEventsProvider) =>
      _states = new Dictionary<Type, IExitState> {
        [typeof(BootstrapState)] = new BootstrapState(this, sceneLoader, globalService),
        [typeof(LoadLevelState)] = new LoadLevelState(globalService.GetSingle<IInputService>(), monoEventsProvider,
          globalService.GetSingle<IGameFactory>(), globalService.GetSingle<IStaticDataService>(), globalService.GetSingle<IUnitSpawner>(),
          sceneLoader)
      };

    public void Enter<TState>() where TState : class, IState {
      IState state = ChangeState<TState>();
      state.Enter();
    }

    public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload> {
      TState state = ChangeState<TState>();
      state.Enter(payload);
    }

    private TState ChangeState<TState>() where TState : class, IExitState {
      _activeState?.Exit();
      TState state = GetState<TState>();
      _activeState = state;
      return state;
    }

    private TState GetState<TState>() where TState : class, IExitState => _states[typeof(TState)] as TState;
  }
}