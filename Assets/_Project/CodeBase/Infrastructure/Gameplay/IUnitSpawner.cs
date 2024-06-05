using CodeBase.Infrastructure.Services;
using CodeBase.Units;

namespace CodeBase.Infrastructure.Gameplay {
  public interface IUnitSpawner : IService {
    void SpawnUnit(UnitType type);
    BehavioursCollection Collection { get; }
    void Construct(GameBoard gameBoard);
  }
}