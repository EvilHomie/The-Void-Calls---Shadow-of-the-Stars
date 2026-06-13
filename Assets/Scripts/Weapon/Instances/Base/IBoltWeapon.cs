using UnityEngine;

namespace Weapons
{
    public interface IBoltWeapon
    {
        public float FireRate { get; }
        public float SpreadAngle { get; }
        public float ProjectileSpeed { get; }
        public Rigidbody2D ShipRB { get; }
        void SetShipRigidbody(Rigidbody2D rb);
    }
}
