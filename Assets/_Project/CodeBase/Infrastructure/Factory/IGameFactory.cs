using System.Threading.Tasks;
using CodeBase.BoardContent;
using CodeBase.Infrastructure.Gameplay;
using CodeBase.Infrastructure.Pool;
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
    TurretBullet CreateBullet();
    void Reclaim(FactoryObject unit);
    void ReclaimBullet(TurretBullet bullet);
    GameOverScreen CreateGameOverScreen();
    Task Clear();
  }
}