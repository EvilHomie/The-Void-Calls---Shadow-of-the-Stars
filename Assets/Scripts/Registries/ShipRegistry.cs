using Ships;
using System.Collections.Generic;

namespace Registries
{
    public class ShipRegistry
    {
        private ShipInstance _playerShip;
        private HashSet<ShipInstance> _otherShips = new(200); // за исключением игрока

        public ShipInstance PlayerShip => _playerShip;
        public IReadOnlyCollection<ShipInstance> OtherShips => _otherShips;

        public void RegisterPlayerShip(ShipInstance shipInstance)
        {
            _playerShip = shipInstance;
        }

        public void RegiserOtherShip(ShipInstance shipInstance)
        {
            _otherShips.Add(shipInstance);
        }

        public void UnRegiserPlayerShip()
        {
            _playerShip = null;
        }

        public void UnRegiserOtherShip(ShipInstance shipInstance)
        {
            _otherShips.Remove(shipInstance);
        }
    }
}