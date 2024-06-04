using CodeBase.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBase.UI {
  public class AccelerationControl : UIBehaviour {
    [SerializeField]
    private Button _accelerationButton;
    [SerializeField]
    private Button _slowingButton;
    private readonly int _maxAcceleration = 3;
    private readonly float _minAcceleration = 0.5f;
    private readonly float _accelerationStep = 0.5f;

    protected override void Awake() {
      _accelerationButton.AddListener(Accelerate);
      _slowingButton.AddListener(Slowing);
    }

    protected override void Start() => UpdateDisplay();

    protected override void OnDestroy() {
      _accelerationButton.RemoveListener(Accelerate);
      _slowingButton.RemoveListener(Slowing);
    }

    private void UpdateDisplay() {
      float timescale = Time.timeScale;

      if (timescale >= _maxAcceleration) {
        Time.timeScale = _maxAcceleration;
        _accelerationButton.interactable = false;
      }

      if (timescale <= _minAcceleration) {
        Time.timeScale = _minAcceleration;
        _slowingButton.interactable = false;
      }
    }

    private void Accelerate() {
      Time.timeScale += _accelerationStep;
      _slowingButton.interactable = true;
      UpdateDisplay();
    }

    private void Slowing() {
      Time.timeScale -= _accelerationStep;
      _accelerationButton.interactable = true;
      UpdateDisplay();
    }
  }
}