using CodeBase.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBase.UI {
  public class WelcomeScreen : UIBehaviour {
    [SerializeField]
    private Button _okButton;
    protected override void Start() => _okButton.AddListener(RemoveWelcomeScreen);
    protected override void OnDestroy() => _okButton.RemoveListener(RemoveWelcomeScreen);

    private void RemoveWelcomeScreen() => Destroy(gameObject);
  }
}