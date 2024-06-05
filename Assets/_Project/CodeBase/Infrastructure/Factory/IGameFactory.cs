using System.Threading.Tasks;
using CodeBase.BoardContent;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Services;
using CodeBase.Tower;
using CodeBase.UI;
using CodeBase.Units;

namespace CodeBase.Infrastructure.Factory {
  public interface IGameFactory : IService {
    HUD CreateHUD();
    GameBoard CreateGameBoard();
    TileContent Create(TileContentType type);
    UnitBase Create(UnitType type);
    Tower.Tower CreateTower(TowerType towerType);
    void Reclaim(FactoryObject unit);
    GameOverScreen CreateGameOverScreen();
    Task Clear();
    WelcomeScreen CreateWelcomeScreen();
  }
}