using UnityEngine;

namespace CodeBase.Units {
  public abstract class GameBehaviour : MonoBehaviour {
    public virtual bool GameUpdate() => true;
    public abstract void Recycle();
  }
}