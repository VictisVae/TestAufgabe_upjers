using CodeBase.Factory;
using UnityEngine;

namespace CodeBase.SceneCreation {
  [SelectionBase]
  public class TileContent : MonoBehaviour {
    [SerializeField]
    private TileContentType _type;
    
    public TileContentType Type => _type;
    
    public TileContentFactory TileContentFactory { get; set; }

    public void Recycle() => TileContentFactory.Reclaim(this);

    public bool IsBlockingPath => Type is TileContentType.Ground or TileContentType.Tower;

    public virtual void GameUpdate() {}
  }
}