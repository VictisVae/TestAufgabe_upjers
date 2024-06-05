using UnityEngine;

namespace CodeBase.Tower {
  public class RadiusObject : MonoBehaviour {
    [SerializeField]
    private MeshRenderer _meshRenderer;
    private Color _initialColor;
    private void Awake() => _initialColor = _meshRenderer.material.color;
    public void SwitchColor(Color color) => _meshRenderer.material.color = color;
    public void ResetColor() => _meshRenderer.material.color = _initialColor;

    public void Enable() => _meshRenderer.enabled = true;
    public void Disable() => _meshRenderer.enabled = false;
  }
}