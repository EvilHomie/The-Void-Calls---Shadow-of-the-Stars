using DI;
using GamePools;
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
        private ShipsPool _shipsPool;

        [SerializeField] ShipId testPlayerShip;
        [SerializeField] ShipId[] testEnemies;

        [Inject]
        public void Construct(ShipRegistry shipRegistry, GameFlowSystem gameFlowSystem, ShipsPool shipsPool)
        {
            _shipRegistry = shipRegistry;
            _gameFlowSystem = gameFlowSystem;
            _shipsPool = shipsPool;
        }

        private void Start() // временный метод для запуска кор логики
        {
            var shipPoolId = _shipsPool.GetPoolIdByShipId(testPlayerShip);

            var playerShipInstance = _shipsPool.Getitem(shipPoolId);
            playerShipInstance.Transform.position = Vector3.zero;


            InitHelper.InitShip(playerShipInstance);
            _shipRegistry.RegisterPlayerShip(playerShipInstance);
            EventBus.PlayerShipSpawned?.Invoke(playerShipInstance);

            SetDepthPos(playerShipInstance);


            foreach (var enemy in testEnemies)
            {
                var enemyShipPoolId = _shipsPool.GetPoolIdByShipId(enemy);
                var enemyInstance = _shipsPool.Getitem(enemyShipPoolId);

                InitHelper.InitShip(enemyInstance);
                enemyInstance.AimData.TargetRigidBody = playerShipInstance.Rigidbody;
                _shipRegistry.RegisterShip(enemyInstance, SimulationLevel.Lod0);
                SetDepthPos(enemyInstance);
            }

            _gameFlowSystem.ChangeGameState(GameState.CoreGameplay);
        }

        private void SetDepthPos(ShipInstance shipInstance)
        {
            var pos = shipInstance.transform.position;
            pos.z = 1;
            shipInstance.transform.position = pos;
        }
    }    
}

