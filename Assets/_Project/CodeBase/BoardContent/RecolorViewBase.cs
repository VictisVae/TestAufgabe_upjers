using System;
using UnityEngine;

namespace CodeBase.BoardContent {
  [Serializable]
  public abstract class RecolorViewBase {
    public abstract void SetTileType(TileContentType type);
    public abstract void ViewAvailable(bool isAvailable);
    public abstract void SetNormal();
  }
  
  [Serializable]
  public class TileRecolor : RecolorViewBase {
    [SerializeField]
    private MeshRenderer _meshRenderer;

    private TileContentType _type = TileContentType.Empty;
    
    public override void SetTileType(TileContentType type) => _type = type;

    public override void ViewAvailable(bool isAvailable) {
      if (_type == TileContentType.Empty) {
        return;
      }

      _meshRenderer.material.color = isAvailable ? Color.green : Color.red;
    }

    public override void SetNormal() {
      if (_type == TileContentType.Empty) {
        return;
      }

      _meshRenderer.material.color = Color.white;
    }
  }
  
  [Serializable]
  public class TowerRecolor : RecolorViewBase {
    [SerializeField]
    private MeshRenderer[] _meshRenderers;

    public override void SetTileType(TileContentType type) {}

    public override void ViewAvailable(bool isAvailable) {
      foreach (var meshRenderer in _meshRenderers) {
        meshRenderer.material.color = isAvailable ? Color.green : Color.red;
      }
    }

    public override void SetNormal() {
      foreach (var meshRenderer in _meshRenderers) {
        meshRenderer.material.color = Color.white;
      }
    }
  }
}