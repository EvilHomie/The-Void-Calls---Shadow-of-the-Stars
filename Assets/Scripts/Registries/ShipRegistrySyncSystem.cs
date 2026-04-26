using DI;
using Registries;
using Ships;
using System.Collections.Generic;

namespace GameSystems
{
    public class ShipRegistrySyncSystem : GameSystemBase
    {
        private ShipRegistry _shipRegystry;

        private HashSet<ShipInstance> _shipsToAdd = new(20);
        private HashSet<ShipInstance> _shipsToRemove = new(20);


        [Inject]
        public void Construct(ShipRegistry shipRegystry)
        {
            _shipRegystry = shipRegystry;
        }

        protected override void AwakeInit()
        {
        }

        protected override void Subscribe()
        {
            EventBus.SpawnPlayerShip += OnPlayerShipSpawned;
            EventBus.RemovePlayerShip += OnPlayerShipDestroyed;
            EventBus.SpawnOtherShip += OnOtherShipSpawned;
            EventBus.RemoveOtherShip += OnOtherShipDestroyed;
            GameFlowSystem.PreUpdateTick += SyncShipRegistry;
        }

        protected override void Unsubscribe()
        {
            EventBus.SpawnPlayerShip -= OnPlayerShipSpawned;
            EventBus.RemovePlayerShip -= OnPlayerShipDestroyed;
            EventBus.SpawnOtherShip -= OnOtherShipSpawned;
            EventBus.RemoveOtherShip -= OnOtherShipDestroyed;
            GameFlowSystem.PreUpdateTick -= SyncShipRegistry;
        }

        private void OnPlayerShipSpawned(ShipInstance shipInstance)
        {
            _shipRegystry.RegisterPlayerShip(shipInstance);
        }

        private void OnPlayerShipDestroyed()
        {
            _shipRegystry.UnRegiserPlayerShip();
        }

        private void OnOtherShipSpawned(ShipInstance shipInstance)
        {
            _shipsToRemove.Remove(shipInstance);
            _shipsToAdd.Add(shipInstance);
        }

        private void OnOtherShipDestroyed(ShipInstance shipInstance)
        {
            _shipsToAdd.Remove(shipInstance);
            _shipsToRemove.Add(shipInstance);
        }

        private void SyncShipRegistry()
        {
            foreach (var ship in _shipsToRemove)
            {
                _shipRegystry.UnRegiserOtherShip(ship);
            }

            _shipsToRemove.Clear();

            foreach (var ship in _shipsToAdd)
            {
                _shipRegystry.RegiserOtherShip(ship);
            }

            _shipsToAdd.Clear();
        }
    }
}