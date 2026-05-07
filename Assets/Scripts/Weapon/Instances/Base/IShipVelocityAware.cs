using UnityEngine;

namespace Weapons
{
    internal interface IShipVelocityAware
    {
        public Rigidbody2D ShipRB { get; }
        void SetShipRigidbody(Rigidbody2D rb);
    }
}
