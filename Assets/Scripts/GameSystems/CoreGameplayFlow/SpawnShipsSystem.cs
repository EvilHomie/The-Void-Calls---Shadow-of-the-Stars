using DI;
using General;
using Helpers;
using Registries;
using Ships;
using UnityEngine;

namespace CoreGameSystems
{
    public class SpawnShipsSystem : MonoBehaviour
    {
        private ShipRegistry _shipRegistry;
        private GameFlowSystem _gameFlowSystem;

        [SerializeField] ShipInstance testPlayerShip;
        [SerializeField] ShipInstance[] testEnemies;

        [Inject]
        public void Construct(ShipRegistry shipRegistry, GameFlowSystem gameFlowSystem)
        {
            _shipRegistry = shipRegistry;
            _gameFlowSystem = gameFlowSystem;
        }

        private void Start() // временный метод для запуска кор логики
        {
            InitHelper.InitShip(testPlayerShip);
            _shipRegistry.RegisterPlayerShip(testPlayerShip);
            EventBus.PlayerShipSpawned?.Invoke(testPlayerShip);

            foreach (var ship in testEnemies)
            {
                InitHelper.InitShip(ship);
                ship.AimData.TargetRigidBody = testPlayerShip.Rigidbody;
                _shipRegistry.RegisterShip(ship, SimulationLevel.Lod0);
            }

            _gameFlowSystem.ChangeGameState(GameState.CoreGameplay);
        }
    }
}

