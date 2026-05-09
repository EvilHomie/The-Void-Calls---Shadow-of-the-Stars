using DI;
using Helpers;
using Registries;
using Ships;

namespace GameSystems
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

        private void Start()
        {
            InitHelper.InitShip(_playerShip);
            InitHelper.InitWeapons(_playerShip);
            InitHelper.RegisterShip(_playerShip, _shipRegistry, asPlayer: true);
            _gameFlowSystem.ChangeGameState(GameState.CoreGameplay);
        }
    }
}

