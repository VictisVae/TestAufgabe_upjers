using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.Random;
using CodeBase.SceneCreation;
using CodeBase.Units;

namespace CodeBase.Infrastructure.Gameplay {
  public class UnitSpawner : IUnitSpawner {
    private readonly IRandomService _randomService;
    private readonly IGameFactory _factory;
    private GameBoard _board;

    public UnitSpawner(IRandomService randomService, IGameFactory factory) {
      _randomService = randomService;
      _factory = factory;
    }

    public void SpawnUnit(UnitType type) {
      BoardTile spawnPoint = _board.GetSpawnPoint(_randomService.Range(0, _board.SpawnPointCount));
      BoardTile destinationPoint = _board.GetDestinationPoints(_randomService.Range(0, _board.DestinationPointCount));
      UnitBase unitBase = _factory.Create(type);

      if (unitBase is AirUnit airUnit) {
        airUnit.SpawnItOn(spawnPoint, destinationPoint);
      } else {
        unitBase.SpawnItOn(spawnPoint);
      }

      Collection.Add(unitBase);
    }

    public void Construct(GameBoard gameBoard) => _board = gameBoard;
    public BehavioursCollection Collection { get; } = new BehavioursCollection();
  }
}