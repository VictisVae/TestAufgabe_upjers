namespace CodeBase.Infrastructure.States {
  public interface IState : IExitState {
    void Enter();
  }

  public interface IPayloadState<in TPayload> : IExitState {
    void Enter(TPayload payload);
  }

  public interface IExitState {
    void Exit();
  }
}