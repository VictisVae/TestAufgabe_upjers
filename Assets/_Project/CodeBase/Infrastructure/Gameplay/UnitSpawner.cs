using CodeBase.Grid;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.Random;
using CodeBase.Units;

namespace CodeBase.Infrastructure.Gameplay {
  public class UnitSpawner : IUnitSpawner {
    private readonly IRandomService _randomService;
    private readonly IGameFactory _factory;
    private GridController _gridController;

    public UnitSpawner(IRandomService randomService, IGameFactory factory) {
      _randomService = randomService;
      _factory = factory;
    }

    public void SpawnUnit(UnitType type) {
      GridTile spawnPoint = _gridController.GetSpawnPoint(_randomService.Range(0, _gridController.SpawnPointCount));
      GridTile destinationPoint = _gridController.GetDestinationPoints(_randomService.Range(0, _gridController.DestinationPointCount));
      UnitBase unitBase = _factory.Create(type);

      if (unitBase is AirUnit airUnit) {
        airUnit.SpawnItOn(spawnPoint, destinationPoint);
      } else {
        unitBase.SpawnItOn(spawnPoint);
      }

      Collection.Add(unitBase);
    }

    public void Construct(GridController gridController) => _gridController = gridController;
    public BehavioursCollection Collection { get; } = new BehavioursCollection();
  }
}