using DI;
using GameCamera;
using Registries;

namespace GameSystems
{
    public class TargetSystem : GameSystemBase, ICorePreUpdateTickObserver
    {
        private ShipRegistry _shipRegistry;
        private MouseCursor _mouseCursor;

        [Inject]
        public void Construct(ShipRegistry shipRegistry, MouseCursor mouseCursor)
        {
            _mouseCursor = mouseCursor;
            _shipRegistry = shipRegistry;
        }

        public void CorePreUpdateTick()
        {
            var playerShip = _shipRegistry.PlayerShip;
            playerShip.TargetData.ShootPosition = _mouseCursor.WorldPostition;

            foreach (var ship in _shipRegistry.ShipsInFight)
            {
                var shipTargetData = ship.TargetData;
                shipTargetData.ShootPosition = shipTargetData.Transform.position;
            }
        }
    }
}

