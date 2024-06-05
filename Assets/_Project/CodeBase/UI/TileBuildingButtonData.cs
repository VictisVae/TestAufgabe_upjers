using CodeBase.BoardContent;
using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.UI {
  public class TileBuildingButtonData : BuildingButtonBase {
    [field: SerializeField]
    public TileContentType Type { get; private set; }
    
    private bool _isLimited;
    private int _limit;
    private int _useCounts = 1;
    protected override void Awake() => MeshButtonName.text = $"Build {Type}";
    protected override void Start() => Button.AddListener(WriteUse);

    private void WriteUse() {
      _useCounts++;
      
      if (_isLimited && LimitOverexcited) {
        Button.interactable = false;
      }
    }

    public override void UpdateButtonAvailability(int playerGold) {
      if (playerGold < _costValue && _bringsGold == false) {
        Button.interactable = false;
        return;
      }
      
      if (_isLimited && LimitOverexcited) {
        Button.interactable = false;
        return;
      }

      Button.interactable = true;
    }

    public void SetLimit(bool hasUseLimited, int limit) {
      _limit = limit;
      _isLimited = hasUseLimited;
    }

    private bool LimitOverexcited => _useCounts >= _limit;
  }
}