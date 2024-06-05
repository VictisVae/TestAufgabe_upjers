using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBase.UI {
  public abstract class BuildingButtonBase : UIBehaviour {
    [field: SerializeField]
    public Button Button { get; private set; }
    [field: SerializeField]
    public TextMeshProUGUI MeshButtonName { get; protected set; }
    [SerializeField]
    private TextMeshProUGUI _goldMesh;

    protected bool _bringsGold;
    protected int _costValue;

    public void SetBuildValue(int costValue, bool bringsGold, int playerGold) {
      _costValue = costValue;
      _bringsGold = bringsGold;
      string text = bringsGold ? "Brings:" : "Costs:";
      _goldMesh.text = $"{text} {costValue}";
      UpdateButtonAvailability(playerGold);
    }

    public virtual void UpdateButtonAvailability(int playerGold) {
      if (playerGold < _costValue && _bringsGold == false) {
        Button.interactable = false;
        return;
      }

      Button.interactable = true;
    }
  }
}