using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBase.UI {
  public abstract class BuildingButtonBase : UIBehaviour{
    [field: SerializeField]
    public Button Button { get; private set; }
    [field: SerializeField]
    public TextMeshProUGUI TextMesh { get; protected set; }
  }
}