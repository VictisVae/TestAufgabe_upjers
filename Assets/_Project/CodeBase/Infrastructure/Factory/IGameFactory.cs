using CodeBase.BoardContent;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services;
using CodeBase.UI;
using CodeBase.Units;

namespace CodeBase.Infrastructure.Factory {
  public interface IGameFactory : IService {
    HUD CreateHUD();
    GameBoard CreateGameBoard();
    TileContent Create(TileContentType type);
    UnitBase Create(UnitType type);
    void Reclaim(FactoryObject unit);
    Tower CreateTower(TowerType towerType);
  }
}