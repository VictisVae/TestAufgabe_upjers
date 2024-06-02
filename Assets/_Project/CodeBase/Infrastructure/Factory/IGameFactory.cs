using CodeBase.BoardContent;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services;
using CodeBase.Units;

namespace CodeBase.Infrastructure.Factory {
  public interface IGameFactory : IService {
    GameBoard CreateGameBoard();
    TileContent Create(TileContentType type);
    UnitBase Create(UnitType type);
    void Reclaim(FactoryObject unit);
    Tower CreateTower(TowerType towerType);
  }
}