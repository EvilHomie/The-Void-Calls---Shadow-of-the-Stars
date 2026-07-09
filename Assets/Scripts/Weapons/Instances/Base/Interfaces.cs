using UnityEngine;

namespace Weapons
{
    public interface IRigidBodyDependentWeapon
    {
        public Rigidbody2D ShipRB { get; }
        void Init(Rigidbody2D rb);
    }

    public interface IPoolDependentWeapon
    {
        public uint ProjectilePoolId { get; }
        void Init(uint poolId);
    }
}