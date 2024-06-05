using CodeBase.Infrastructure.Factory;

namespace CodeBase.Units {
  public abstract class GameBehaviour : FactoryObject {
    public virtual bool GameUpdate() => true;
    public abstract void Recycle();
  }
}