using System;
using System.Collections.Generic;
using CodeBase.BoardContent;
using CodeBase.Infrastructure.Services.StaticData.TowerData;
using CodeBase.Tower.Armory;
using UnityEngine;

namespace CodeBase.Tower {
  [RequireComponent(typeof(TowerVisualRadius))]
  public class Tower : TileContent {
    [SerializeField]
    private TowerRecolor _towerRecolor;
    [SerializeField]
    private TowerLaser _towerLaser;
    [SerializeField]
    [Range(1f, 10f)]
    private float _targetingRange = 1.5f;
    private TowerVisualRadius _towerRadius;
    private void Awake() => _towerRadius = GetComponent<TowerVisualRadius>();

    public override void GameUpdate() {
      if (_towerLaser.IsTargetAcquired() || _towerLaser.IsTargetTracked()) {
        _towerLaser.Shoot();
      }
    }

    public override void ViewAvailable(bool isAvailable) {
      _towerRecolor.ViewAvailable(isAvailable);
      _towerRadius.SwitchRadiusColor(isAvailable ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f));
    }

    public override void SetNormal() {
      _towerRecolor.SetNormal();
      _towerRadius.ResetRadiusColor();
    }

    public void Construct(TowerConfig config) {
      _towerLaser.Construct(config.ShootFrequency, config.LaserDamage, _targetingRange);
      TowerType = config.BuildScheme.TowerType;
      _towerRecolor.SetTileType(_type);
    }

    public void ShowRadius() => _towerRadius.ShowRadius();
    public void HideRadius() => _towerRadius.HideRadius();
    public void ReceiveTargets(Func<List<Target>> targets) => _towerLaser.ReceiveTargets(targets);
    public new TowerType TowerType { get; private set; }
    public float TargetingRange => _targetingRange;
  }
}