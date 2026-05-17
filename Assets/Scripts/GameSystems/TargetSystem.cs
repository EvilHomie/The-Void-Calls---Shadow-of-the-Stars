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
            var targetData = playerShip.TargetData;
            targetData.TargetPosition = _mouseCursor.WorldPostition;
            targetData.TargetVelocity = playerShip.Rigidbody.linearVelocity;

            foreach (var ship in _shipRegistry.ShipsInFight)
            {
                var shipTargetData = ship.TargetData;
                var targetRigidBody = shipTargetData.TargetRigidBody;
                shipTargetData.TargetPosition = targetRigidBody.position;
                shipTargetData.TargetVelocity = targetRigidBody.linearVelocity;
            }
        }
    }
}

