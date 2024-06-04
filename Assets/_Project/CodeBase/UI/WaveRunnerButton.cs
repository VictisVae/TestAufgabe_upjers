using CodeBase.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBase.UI {
  public class WaveRunnerButton : UIBehaviour {
    [SerializeField]
    private Button _button;
    [SerializeField]
    private TextMeshProUGUI _infoMesh;
    [SerializeField]
    private TextMeshProUGUI _wavesDisplayMesh;
    private int _totalWaves;
    private int _currentWave;
    protected override void Start() => _button.AddListener(SetDisabled);
    protected override void OnDestroy() => _button.RemoveListener(SetDisabled);

    public void UpdateDisplay(int totalWaves) {
      _totalWaves = totalWaves;
      _infoMesh.text = "Run Wave";
      _wavesDisplayMesh.text = $"Wave: {_currentWave}/{totalWaves}";
    }

    public void SetEnabled() => _button.interactable = true;
    public void SubscribeAction(UnityAction action) => _button.AddListener(action);
    public void UnsubscribeAction(UnityAction action) => _button.RemoveListener(action);
    public void UnsubscribeAll() => _button.RemoveAllListeners();

    private void SetDisabled() {
      _button.interactable = false;
      IncreaseWaves();
      UpdateDisplay(_totalWaves);
    }

    private void IncreaseWaves() {
      if (_currentWave + 1 > _totalWaves) {
        return;
      }

      _currentWave++;
    }
  }
}