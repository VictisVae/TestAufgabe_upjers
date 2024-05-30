using CodeBase.Factory;
using UnityEngine;

namespace CodeBase.SceneCreation {
  public class TileContent : MonoBehaviour {
    [SerializeField]
    private TileContentType _type;
    
    public TileContentType Type => _type;
    
    public TileContentFactory TileContentFactory { get; set; }

    public void Recycle() => TileContentFactory.Reclaim(this);
  }
}