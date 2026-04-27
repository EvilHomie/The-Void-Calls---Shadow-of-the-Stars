using Ships;
using System.Collections.Generic;
using UnityEngine;

namespace Registries
{
    public class ShipRegistry : MonoBehaviour, IPreUpdateTickObserver
    {
        public ShipInstance PlayerShip => _playerShip;
        public IReadOnlyCollection<ShipInstance> OtherShips => _otherShips;

        private ShipInstance _playerShip;
        private readonly HashSet<ShipInstance> _otherShips = new(200); // за исключением игрока
        private readonly HashSet<ShipInstance> _shipsToAdd = new(20);
        private readonly HashSet<ShipInstance> _shipsToRemove = new(20);

        public void PreUpdateTick()
        {
            Sync();
        }

        public void RequestAddPlayerShip(ShipInstance shipInstance)
        {
            _playerShip = shipInstance;
        }

        public void RequestRemovePlayerShip(ShipInstance shipInstance)
        {
            _playerShip = null;
        }

        public void RequestAddOtherShip(ShipInstance shipInstance)
        {
            _shipsToRemove.Remove(shipInstance);
            _shipsToAdd.Add(shipInstance);
        }

        public void RequestRemoveOtherShip(ShipInstance shipInstance)
        {
            _shipsToAdd.Remove(shipInstance);
            _shipsToRemove.Add(shipInstance);
        }

        private void Sync()
        {
            foreach (var ship in _shipsToRemove)
            {
                _otherShips.Remove(ship);
            }

            _shipsToRemove.Clear();

            foreach (var ship in _shipsToAdd)
            {
                _otherShips.Add(ship);
            }

            _shipsToAdd.Clear();
        }
    }
}