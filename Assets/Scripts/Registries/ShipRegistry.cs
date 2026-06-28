using DI;
using General;
using Ships;
using System.Collections.Generic;
using UnityEngine;

namespace Registries
{
    public class ShipRegistry : MonoBehaviour, ICorePreUpdateTickObserver
    {
        public ShipInstance PlayerShip => _playerShip;
        public IReadOnlyCollection<ShipInstance> Lod0Ships => _lod0Ships;
        public IReadOnlyCollection<ShipInstance> Lod1Ships => _lod1Ships;

        private ShipInstance _playerShip;

        private readonly HashSet<ShipInstance> _lod0Ships = new(50);
        private readonly HashSet<ShipInstance> _lod1Ships = new(200);

        private readonly Dictionary<SimulationLevel, HashSet<ShipInstance>> _shipsToAdd = new()
        {
            {SimulationLevel.Lod0, new HashSet<ShipInstance>(50) },
            {SimulationLevel.Lod1, new HashSet<ShipInstance>(200) }

        };
        private readonly Dictionary<SimulationLevel, HashSet<ShipInstance>> _shipsToRemove = new()
        {
            {SimulationLevel.Lod0, new HashSet<ShipInstance>(50) },
            {SimulationLevel.Lod1, new HashSet<ShipInstance>(200) }
        };

        [Inject]
        public void Construct(GameFlowSystem gameFlowSystem)
        {
            gameFlowSystem.AddTickObserver(this);
        }

        public void CorePreUpdateTick()
        {
            Sync();
        }

        public void RegisterPlayerShip(ShipInstance shipInstance)
        {
            _playerShip = shipInstance;
        }

        public void UnRegisterPlayerShip()
        {
            _playerShip = null;
        }

        public void RegisterShip(ShipInstance shipInstance, SimulationLevel simulationLevel)
        {
            _shipsToRemove[simulationLevel].Remove(shipInstance);
            _shipsToAdd[simulationLevel].Add(shipInstance);
        }

        public void UnregisterShip(ShipInstance shipInstance, SimulationLevel simulationLevel)
        {
            _shipsToAdd[simulationLevel].Remove(shipInstance);
            _shipsToRemove[simulationLevel].Add(shipInstance);
        }

        public void SetSimulationLevel(ShipInstance shipInstance, SimulationLevel simulationLevel)
        {
            switch (simulationLevel)
            {
                case SimulationLevel.None:
                    UnregisterShip(shipInstance, SimulationLevel.Lod0);
                    UnregisterShip(shipInstance, SimulationLevel.Lod1);
                    break;
                case SimulationLevel.Lod0:
                    UnregisterShip(shipInstance, SimulationLevel.Lod1);
                    RegisterShip(shipInstance, simulationLevel);
                    break;
                case SimulationLevel.Lod1:
                    UnregisterShip(shipInstance, SimulationLevel.Lod0);
                    RegisterShip(shipInstance, simulationLevel);
                    break;
            }
        }

        private void Sync()
        {
            Sync(_lod0Ships, SimulationLevel.Lod0);
            Sync(_lod1Ships, SimulationLevel.Lod1);
        }

        private void Sync(HashSet<ShipInstance> ships, SimulationLevel simulationLevel)
        {
            var shipsToRemove = _shipsToRemove[simulationLevel];
            foreach (var ship in shipsToRemove)
            {
                ships.Remove(ship);
            }
            shipsToRemove.Clear();

            var shipsToAdd = _shipsToAdd[simulationLevel];
            foreach (var ship in shipsToAdd)
            {
                ships.Add(ship);
            }
            shipsToAdd.Clear();
        }
    }

    public enum SimulationLevel
    {
        None,
        Lod0,
        Lod1
    }
}