using DI;
using GameCamera;
using Registries;

namespace GameSystems
{
    public class DefenseLayersSystem : GameSystemBase, ICorePreUpdateTickObserver
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
            //var playerShip = _shipRegistry.PlayerShip;
            //ref var targetData = ref playerShip.TargetData;
            //targetData.Position = _mouseCursor.WorldPostition;

            //foreach (var ship in _shipRegistry.ShipsInFight)
            //{
            //    ref var shipTargetData = ref ship.TargetData;
            //    shipTargetData.Position = shipTargetData.Transform.position;
            //}
        }
    }
}

