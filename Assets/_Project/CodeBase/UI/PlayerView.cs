using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeBase.UI {
  public class PlayerView : UIBehaviour {
    [SerializeField]
    private TextMeshProUGUI _healthMesh;
    [SerializeField]
    private TextMeshProUGUI _goldMesh;
    public void SetCurrentHealth(int value) => _healthMesh.text = value.ToString();
    public void SetCurrentGold(int value) => _goldMesh.text = value.ToString();
  }
}