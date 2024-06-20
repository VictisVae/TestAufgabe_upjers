﻿using System;
using System.Threading.Tasks;
using CodeBase.BoardContent;
using CodeBase.Grid;
using CodeBase.Infrastructure.Services;
using CodeBase.Towers;
using CodeBase.UI;
using CodeBase.Units;

namespace CodeBase.Infrastructure.Factory {
  public interface IGameFactory : IService {
    HUD CreateHUD();
    GameGridView CreateGameGrid();
    TileContent Create(TileContentType type);
    UnitBase Create(UnitType type);
    Tower CreateTower(TowerType towerType);
    GameOverScreen CreateGameOverScreen();
    void Reclaim(FactoryObject item, Enum itemType);
    Task Clear();
    WelcomeScreen CreateWelcomeScreen();
    void WarmUp();
  }
}