﻿using CodeBase.Utilities;
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
    private TextMeshProUGUI _textMeshPro;
    protected override void Start() => Update();
    public void Update() => _textMeshPro.text = $"Run Wave";
    public void SetEnabled() => _button.interactable = true;
    public void SetDisabled() => _button.interactable = false;
    public void SubscribeAction(UnityAction action) => _button.AddListener(action);
    public void UnsubscribeAction(UnityAction action) => _button.RemoveListener(action);
    public void WaveCompleted() => _textMeshPro.text = $"Run next Wave";
  }
}