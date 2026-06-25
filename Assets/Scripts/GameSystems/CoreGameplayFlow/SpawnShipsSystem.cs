using DI;
using General;
using Helpers;
using Registries;
using Ships;

namespace CoreGameSystems
{
    public class SpawnShipsSystem : GameSystemBase
    {
        private ShipInstance _playerShip;
        private ShipRegistry _shipRegistry;
        private WeaponRegistry _weaponRegistry;
        private GameFlowSystem _gameFlowSystem;

        [Inject]
        public void Construct(ShipInstance playerShip, ShipRegistry shipRegistry, WeaponRegistry weaponRegistry, GameFlowSystem gameFlowSystem)
        {
            _playerShip = playerShip;
            _shipRegistry = shipRegistry;
            _weaponRegistry = weaponRegistry;
            _gameFlowSystem = gameFlowSystem;
        }

        private void Start() // временный метод для запуска кор логики
        {
            InitHelper.InitShip(_playerShip);
            InitHelper.RegisterShip(_playerShip, _shipRegistry, asPlayer: true);
            EventBus.PlayerShipSpawned?.Invoke(_playerShip);
            _gameFlowSystem.ChangeGameState(GameState.CoreGameplay);
        }
    }
}

