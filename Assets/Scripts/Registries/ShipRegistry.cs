using Ships;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Registries
{
    public class ShipRegistry : MonoBehaviour, IPreUpdateTickObserver
    {
        public ShipInstance PlayerShip => _playerShip;
        public IReadOnlyCollection<ShipInstance> OtherShips => _ships;
        public IReadOnlyCollection<ShipInstance> ShipsInFight => _shipsInFight;

        private ShipInstance _playerShip;
        private readonly HashSet<ShipInstance> _ships = new(200); // за исключением игрока
        private readonly HashSet<ShipInstance> _shipsToAdd = new(20);
        private readonly HashSet<ShipInstance> _shipsToRemove = new(20);

        private readonly HashSet<ShipInstance> _shipsInFight = new(200); // за исключением игрока
        private readonly HashSet<ShipInstance> _shipsInFightToAdd = new(20);
        private readonly HashSet<ShipInstance> _shipsInFightToRemove = new(20);

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

        public void RequestRemoveOnExitFight(ShipInstance shipInstance)
        {
            _shipsInFightToAdd.Remove(shipInstance);
            _shipsInFightToRemove.Add(shipInstance);
        }

        public void RequestAddOnEnterFight(ShipInstance shipInstance)
        {
            _shipsInFightToRemove.Remove(shipInstance);
            _shipsInFightToAdd.Add(shipInstance);
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
                _ships.Remove(ship);
            }

            _shipsToRemove.Clear();

            foreach (var ship in _shipsToAdd)
            {
                _ships.Add(ship);
            }

            _shipsToAdd.Clear();
        }
    }
}