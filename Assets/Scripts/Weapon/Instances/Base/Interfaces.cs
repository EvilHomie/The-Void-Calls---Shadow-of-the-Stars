using UnityEngine;

namespace Weapons
{
    public interface IRigidBodyDependentWeapon
    {
        public Rigidbody2D ShipRB { get; }
        void Init(Rigidbody2D rb);
    }
}