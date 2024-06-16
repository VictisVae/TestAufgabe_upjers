﻿using System;
using System.Collections.Generic;
using CodeBase.BoardContent;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.StaticData.TowerData;
using CodeBase.Towers.Armory;
using UnityEngine;

namespace CodeBase.Towers {
  [RequireComponent(typeof(TowerVisualRadius))]
  public class Tower : FactoryObject, ITower {
    [SerializeField]
    private TowerRecolor _towerRecolor;
    [SerializeField]
    private TowerLaser _towerLaser;
    [SerializeField]
    [Range(1f, 10f)]
    private float _targetingRange = 1.5f;
    private TowerVisualRadius _towerRadius;
    private IGameFactory _gameFactory;
    private void Awake() => _towerRadius = GetComponent<TowerVisualRadius>();

    public void GameUpdate() {
      if (_towerLaser.IsTargetAcquired() || _towerLaser.IsTargetTracked()) {
        _towerLaser.Shoot();
      }
    }

    public TowerType TowerType { get; private set; }

    public override void ViewAvailable(bool isAvailable) {
      _towerRecolor.ViewAvailable(isAvailable);
      _towerRadius.SwitchRadiusColor(isAvailable ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f));
    }

    public override void SetNormal() {
      _towerRecolor.SetNormal();
      _towerRadius.ResetRadiusColor();
    }

    public void Construct(IGameFactory gameFactory, TowerConfig config) {
      _gameFactory = gameFactory;
      _towerLaser.Construct(config.ShootFrequency, config.LaserDamage, _targetingRange);
      TowerType = config.BuildScheme.TowerType;
    }
    
    public override void Recycle() => _gameFactory.Reclaim(this, TowerType);
    public void ShowRadius() => _towerRadius.ShowRadius();
    public void HideRadius() => _towerRadius.HideRadius();
    public void ReceiveTargets(Func<List<Target>> targets) => _towerLaser.ReceiveTargets(targets);
    public float TargetingRange => _targetingRange;
  }
}