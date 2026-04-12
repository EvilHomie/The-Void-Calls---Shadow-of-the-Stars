using DI;
using Ships;

namespace GameSystems
{
    public class SpawnShipsSystem : GameSystemBase
    {
        private ShipInstance _playerShip;
        [Inject]
        public void Construct(ShipInstance playerShip)
        {
            _playerShip = playerShip;
        }

        protected override void AwakeInit()
        {
            //QualitySettings.vSyncCount = 0;
        }

        protected override void Subscribe()
        {

        }

        protected override void Unsubscribe()
        {

        }

        private void Start()
        {
            EventBus.SpawnPlayerShip?.Invoke(_playerShip);
            EventBus.GameStateChangeAction?.Invoke(GameState.CoreGameplay);
        }
    }
}

